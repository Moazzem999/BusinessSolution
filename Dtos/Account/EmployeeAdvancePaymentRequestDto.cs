namespace BusinessSolution.Dtos.Account
{
    public class EmployeeAdvancePaymentRequestDto
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
