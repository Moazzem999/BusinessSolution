using BusinessSolution.Dtos.User;
using BusinessSolution.Entities;
using BusinessSolution.Entities.Context;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Shared.Enum;
using BusinessSolution.Shared.Infrastructure;
using BusinessSolution.Shared.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace BusinessSolution.Repositories
{
    public class UsersRepository(AppDbContext context, IConfiguration config) : IUsersRepository
    {
        private readonly AppDbContext context = context;
        private readonly IConfiguration config = config;

        public async Task<Result<long>> Create(UserRequestDto dto)
        {
            if (dto is null)
                return await Result<long>.BadRequestAsync($"Invalid request.");

            if (dto.UserName == string.Empty || dto.Password == string.Empty)
                return await Result<long>.BadRequestAsync($"Username or Password can not be empty.");

            var isValidEmail = Helper.IsValidEmail(dto.Email);
            if (isValidEmail == false)
                return await Result<long>.BadRequestAsync($"Please provide valid email address.");

            var existingUser = await context.Users.AsNoTracking()
                .Where(x => x.UserName == dto.UserName || x.Email == dto.Email)
                .Select(x => new { x.UserName, x.Email })
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                if (existingUser.UserName == dto.UserName)
                    return await Result<long>.BadRequestAsync("Username already taken.");

                if (existingUser.Email == dto.Email)
                    return await Result<long>.BadRequestAsync("Email already taken.");
            }

            // BCrypt.HashPassword generates a random salt and incorporates it into the hash string
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var newEntity = new UserEntity
            {
                Name = dto.Name,
                Email = dto.Email,
                UserName = dto.UserName,
                Password = passwordHash,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                Status = Status.Active
            };

            context.Users.Add(newEntity);
            await context.SaveChangesAsync();

            return await Result<long>.SuccessAsync($"User successfully created.", newEntity.Id);
        }

        public async Task<Result<List<UserDto>>> GetAllUsers()
        {
            var data = await context.Users.AsNoTracking()
                .Where(x => x.Status == Status.Active).ToListAsync();

            var users = data.Select(x => new UserDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                UserName = x.UserName,
                Password = x.Password,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                Status = x.Status
            }).ToList();

            return await Result<List<UserDto>>.SuccessAsync("", users);
        }

        public async Task<Result<LoginResponseDto>> Login(LoginRequestDto dto)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.UserName == dto.UserName && u.Status == Status.Active);

            if (user == null || !Helper.VerifyPassword(dto.Password, user.Password))
                return await Result<LoginResponseDto>.ErrorAsync($"Please provide valid credentials.", (int)HttpStatusCode.Unauthorized);

            // Generate JWT
            var token = GenerateJwtToken(user);

            var response = new LoginResponseDto
            {
                Token = token,
                ExpiresIn = 30,
                User = new UserLoginResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                }
            };

            return await Result<LoginResponseDto>.SuccessAsync("", response);
        }

        private string GenerateJwtToken(UserEntity user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? user.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}