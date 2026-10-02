using BusinessSolution.Dtos.User;
using BusinessSolution.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSolution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUsersRepository usersRepository) : ControllerBase
    {
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var data = await usersRepository.Login(dto);
            return Ok(data);
        }
    }
}
