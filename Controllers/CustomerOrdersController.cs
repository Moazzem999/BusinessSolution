using BusinessSolution.Dtos.CustomerOrder;
using BusinessSolution.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSolution.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerOrdersController(ICustomerOrdersService customerOrdersService) : ControllerBase
    {
        private readonly ICustomerOrdersService customerOrdersService = customerOrdersService;

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] CustomerOrderSearchDto searchDto)
        {
            var result = await customerOrdersService.GetAll(searchDto);
            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await customerOrdersService.GetById(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CustomerOrderRequestDto dto)
        {
            var result = await customerOrdersService.Create(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await customerOrdersService.Delete(id);
            return Ok(result);
        }
    }
}

