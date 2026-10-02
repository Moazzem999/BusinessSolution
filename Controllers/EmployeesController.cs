using BusinessSolution.Dtos.Employee;
using BusinessSolution.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSolution.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController(IEmployeesService employeesService) : ControllerBase
    {
        private readonly IEmployeesService employeesService = employeesService;

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await employeesService.GetAll();
            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await employeesService.GetById(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EmployeeRequestDto dto)
        {
            var result = await employeesService.Create(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] EmployeeRequestDto dto)
        {
            var result = await employeesService.Update(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await employeesService.Delete(id);
            return Ok(result);
        }
    }
}
