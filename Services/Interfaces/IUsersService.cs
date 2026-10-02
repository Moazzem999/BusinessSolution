using BusinessSolution.Dtos.User;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services.Interfaces
{
    public interface IUsersService
    {
        Task<Result<List<UserDto>>> GetAllUsers();
        Task<Result<long>> Create(UserRequestDto dto);
        Task<Result<LoginResponseDto>> Login(LoginRequestDto dto);
    }
}
