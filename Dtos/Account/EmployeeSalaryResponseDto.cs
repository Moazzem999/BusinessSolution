using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.Account
{
    public class EmployeeSalaryResponseDto : BaseDto
    {
        public long EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? Designation { get; set; }
        public string? ImagePath { get; set; }
        public decimal Salary { get; set; }
        public decimal AdvancePayment { get; set; }
        public decimal BonusPayment { get; set; }
        public decimal OthersBill { get; set; }
        public decimal Total { get; set; }
        public string PaySlipFor { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }
}
