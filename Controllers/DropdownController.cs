using BusinessSolution.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSolution.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DropdownController(IDropdownService dropdownService) : ControllerBase
    {
        private readonly IDropdownService dropdownService = dropdownService;

        [HttpGet("GetAllMaritalStatus")]
        public async Task<IActionResult> GetAllMaritalStatus()
        {
            var result = await dropdownService.GetAllMaritalStatus();
            return Ok(result);
        }

        [HttpGet("GetAllReligion")]
        public async Task<IActionResult> GetAllReligion()
        {
            var result = await dropdownService.GetAllReligion();
            return Ok(result);
        }
    }
}
