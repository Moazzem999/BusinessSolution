using BusinessSolution.Dtos.CustomerTransactionHistory;
using BusinessSolution.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSolution.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerTransactionHistoriesController(ICustomerTransactionHistoriesService customerTransactionHistoriesService) : ControllerBase
    {
        private readonly ICustomerTransactionHistoriesService customerTransactionHistoriesService = customerTransactionHistoriesService;

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] CustomerTransactionHistorySearchDto searchDto)
        {
            var result = await customerTransactionHistoriesService.GetAll(searchDto);
            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await customerTransactionHistoriesService.GetById(id);
            return Ok(result);
        }
    }
}

