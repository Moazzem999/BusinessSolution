using BusinessSolution.Dtos.SupplierTransactionHistory;
using BusinessSolution.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSolution.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierTransactionHistoriesController(ISupplierTransactionHistoriesService supplierTransactionHistoriesService) : ControllerBase
    {
        private readonly ISupplierTransactionHistoriesService supplierTransactionHistoriesService = supplierTransactionHistoriesService;

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] SupplierTransactionHistorySearchDto searchDto)
        {
            var result = await supplierTransactionHistoriesService.GetAll(searchDto);
            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await supplierTransactionHistoriesService.GetById(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SupplierTransactionHistoryRequestDto dto)
        {
            var result = await supplierTransactionHistoriesService.Create(dto);
            return Ok(result);
        }
    }
}
