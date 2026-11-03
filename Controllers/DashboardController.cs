using BusinessSolution.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSolution.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController(IDashboardService dashboardService) : ControllerBase
    {
        private readonly IDashboardService dashboardService = dashboardService;

        [HttpGet("TotalSpecialCustomerCount")]
        public async Task<IActionResult> TotalSpecialCustomerCount()
        {
            var result = await dashboardService.TotalSpecialCustomerCount();
            return Ok(result);
        }

        [HttpGet("TotalRegularCustomerCount")]
        public async Task<IActionResult> TotalRegularCustomerCount()
        {
            var result = await dashboardService.TotalRegularCustomerCount();
            return Ok(result);
        }

        [HttpGet("TotalSalesToday")]
        public async Task<IActionResult> TotalSalesToday()
        {
            var result = await dashboardService.TotalSalesToday();
            return Ok(result);
        }

        [HttpGet("TotalPaymentReceivedToday")]
        public async Task<IActionResult> TotalPaymentReceivedToday()
        {
            var result = await dashboardService.TotalPaymentReceivedToday();
            return Ok(result);
        }

        [HttpGet("TotalSalesThisMonth")]
        public async Task<IActionResult> TotalSalesThisMonth()
        {
            var result = await dashboardService.TotalSalesThisMonth();
            return Ok(result);
        }

        [HttpGet("TotalPaymentReceivedThisMonth")]
        public async Task<IActionResult> TotalPaymentReceivedThisMonth()
        {
            var result = await dashboardService.TotalPaymentReceivedThisMonth();
            return Ok(result);
        }

        [HttpGet("TotalSalesThisYear")]
        public async Task<IActionResult> TotalSalesThisYear()
        {
            var result = await dashboardService.TotalSalesThisYear();
            return Ok(result);
        }

        [HttpGet("TotalPaymentReceivedThisYear")]
        public async Task<IActionResult> TotalPaymentReceivedThisYear()
        {
            var result = await dashboardService.TotalPaymentReceivedThisYear();
            return Ok(result);
        }
    }
}
