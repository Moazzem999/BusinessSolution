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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EmployeeAdvancePaymentRequestDto dto)
        {
            var result = await accountsService.CreateEmployeeAdvancePayment(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] EmployeeAdvancePaymentRequestDto dto)
        {
            var result = await accountsService.UpdateEmployeeAdvancePayment(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await accountsService.DeleteEmployeeAdvancePayment(id);
            return Ok(result);
        }
    }
}
