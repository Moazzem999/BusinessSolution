using BusinessSolution.Dtos.Customer;
using BusinessSolution.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSolution.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController(ICustomersService customersService) : ControllerBase
    {
        private readonly ICustomersService customersService = customersService;

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] CustomerSearchDto searchDto)
        {
            var result = await customersService.GetAll(searchDto);
            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await customersService.GetById(id);
            return Ok(result);
        }

        [HttpGet("GetByName/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var result = await customersService.GetByName(name);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] CustomerRequestDto dto)
        {
            var result = await customersService.Create(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromForm] CustomerRequestDto dto)
        {
            var result = await customersService.Update(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await customersService.Delete(id);
            return Ok(result);
        }
    }
}

