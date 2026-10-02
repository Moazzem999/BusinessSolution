using BusinessSolution.Dtos.User;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Services.Interfaces;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services
{
    public class UsersService(IUsersRepository usersRepository) : IUsersService
    {
        public async Task<Result<List<UserDto>>> GetAllUsers()
        {
            return await usersRepository.GetAllUsers();
        }

        public async Task<Result<long>> Create(UserRequestDto dto)
        {
            return await usersRepository.Create(dto);
        }

        public async Task<Result<LoginResponseDto>> Login(LoginRequestDto dto)
        {
            return await usersRepository.Login(dto);
        }
    }
}
