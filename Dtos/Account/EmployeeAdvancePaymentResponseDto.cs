using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.Account
{
    public class EmployeeAdvancePaymentResponseDto : BaseDto
    {
        public long EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? ImagePath { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
