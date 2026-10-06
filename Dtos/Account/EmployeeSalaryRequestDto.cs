namespace BusinessSolution.Dtos.Account
{
    public class EmployeeSalaryRequestDto
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public decimal Salary { get; set; }
        public decimal AdvancePayment { get; set; }
        public decimal BonusPayment { get; set; }
        public decimal OthersBill { get; set; }
        public decimal Total { get; set; }
        public string PaySlipFor { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }
}
