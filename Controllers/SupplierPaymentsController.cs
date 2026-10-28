using BusinessSolution.Dtos.SupplierPayment;
using BusinessSolution.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSolution.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierPaymentsController(ISupplierPaymentsService supplierPaymentsService) : ControllerBase
    {
        private readonly ISupplierPaymentsService supplierPaymentsService = supplierPaymentsService;

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] SupplierPaymentSearchDto searchDto)
        {
            var result = await supplierPaymentsService.GetAll(searchDto);
            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await supplierPaymentsService.GetById(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SupplierPaymentRequestDto dto)
        {
            var result = await supplierPaymentsService.Create(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await supplierPaymentsService.Delete(id);
            return Ok(result);
        }
    }
}

