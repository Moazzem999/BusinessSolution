using BusinessSolution.Dtos.CustomerPayment;
using BusinessSolution.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSolution.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerPaymentsController(ICustomerPaymentsService customerPaymentsService) : ControllerBase
    {
        private readonly ICustomerPaymentsService customerPaymentsService = customerPaymentsService;

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] CustomerPaymentSearchDto searchDto)
        {
            var result = await customerPaymentsService.GetAll(searchDto);
            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await customerPaymentsService.GetById(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CustomerPaymentRequestDto dto)
        {
            var result = await customerPaymentsService.Create(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await customerPaymentsService.Delete(id);
            return Ok(result);
        }
    }
}

