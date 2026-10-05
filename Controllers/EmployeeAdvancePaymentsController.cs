using BusinessSolution.Dtos.Account;
using BusinessSolution.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSolution.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAdvancePaymentsController(IEmployeeAdvancePaymentsService employeeAdvancePaymentsService) : ControllerBase
    {
        private readonly IEmployeeAdvancePaymentsService employeeAdvancePaymentsService = employeeAdvancePaymentsService;

        [HttpGet("GetAllEmployeeAdvancePayments")]
        public async Task<IActionResult> GetAllEmployeeAdvancePayments([FromQuery] EmployeeAdvancePaymentSearchDto searchDto)
        {
            var result = await employeeAdvancePaymentsService.GetAllEmployeeAdvancePayments(searchDto);
            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await employeeAdvancePaymentsService.GetById(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EmployeeAdvancePaymentRequestDto dto)
        {
            var result = await employeeAdvancePaymentsService.CreateEmployeeAdvancePayment(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] EmployeeAdvancePaymentRequestDto dto)
        {
            var result = await employeeAdvancePaymentsService.UpdateEmployeeAdvancePayment(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await employeeAdvancePaymentsService.DeleteEmployeeAdvancePayment(id);
            return Ok(result);
        }
    }
}
