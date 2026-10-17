using BusinessSolution.Dtos.SupplierOrder;
using BusinessSolution.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSolution.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierOrdersController(ISupplierOrdersService supplierOrdersService) : ControllerBase
    {
        private readonly ISupplierOrdersService supplierOrdersService = supplierOrdersService;

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] SupplierOrderSearchDto searchDto)
        {
            var result = await supplierOrdersService.GetAll(searchDto);
            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await supplierOrdersService.GetById(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SupplierOrderRequestDto dto)
        {
            var result = await supplierOrdersService.Create(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] SupplierOrderRequestDto dto)
        {
            var result = await supplierOrdersService.Update(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await supplierOrdersService.Delete(id);
            return Ok(result);
        }
    }
}
