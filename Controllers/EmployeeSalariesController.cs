using BusinessSolution.Dtos.Account;
using BusinessSolution.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSolution.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeSalariesController(IEmployeeSalariesService employeeSalariesService) : ControllerBase
    {
        private readonly IEmployeeSalariesService employeeSalariesService = employeeSalariesService;

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] EmployeeSalarySearchDto searchDto)
        {
            var result = await employeeSalariesService.GetAll(searchDto);
            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await employeeSalariesService.GetById(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EmployeeSalaryRequestDto dto)
        {
            var result = await employeeSalariesService.Create(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] EmployeeSalaryRequestDto dto)
        {
            var result = await employeeSalariesService.Update(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await employeeSalariesService.Delete(id);
            return Ok(result);
        }
    }
}
