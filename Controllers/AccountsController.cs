using BusinessSolution.Dtos.Account;
using BusinessSolution.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSolution.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController(IAccountsService accountsService) : ControllerBase
    {
        private readonly IAccountsService accountsService = accountsService;

        [HttpGet("GetAllEmployeeAdvancePayments")]
        public async Task<IActionResult> GetAllEmployeeAdvancePayments([FromQuery] EmployeeAdvancePaymentSearchDto searchDto)
        {
            var result = await accountsService.GetAllEmployeeAdvancePayments(searchDto);
            return Ok(result);
        }
    }
}
