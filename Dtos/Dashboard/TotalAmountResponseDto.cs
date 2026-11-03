namespace BusinessSolution.Dtos.Dashboard
{
    public class TotalAmountResponseDto
    {
        public decimal TotalAmount { get; set; }

        public TotalAmountResponseDto() { }

        public TotalAmountResponseDto(decimal totalAmount)
        {
            TotalAmount = totalAmount;
        }
    }
}

