using BusinessSolution.Dtos.User;
using BusinessSolution.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSolution.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUsersRepository usersRepository) : ControllerBase
    {
        private readonly IUsersRepository usersRepository = usersRepository;

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await usersRepository.GetAllUsers();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserRequestDto dto)
        {
            var data = await usersRepository.Create(dto);
            return Ok(data);
        }
    }
}
