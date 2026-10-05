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
        public async Task<IActionResult> GetAll([FromQuery] EmployeeSearchDto searchDto)
        {
            var result = await employeesService.GetAll(searchDto);
            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await employeesService.GetById(id);
            return Ok(result);
        }

        [HttpGet("GetByName/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var result = await employeesService.GetByName(name);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] EmployeeRequestDto dto)
        {
            var result = await employeesService.Create(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromForm] EmployeeRequestDto dto)
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
