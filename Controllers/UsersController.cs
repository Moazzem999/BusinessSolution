using BusinessSolution.Dtos.User;
using BusinessSolution.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSolution.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUsersService userService) : ControllerBase
    {
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await userService.GetAllUsers();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserRequestDto dto)
        {
            var data = await userService.Create(dto);
            return Ok(data);
        }
    }
}
