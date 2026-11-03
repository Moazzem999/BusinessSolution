namespace BusinessSolution.Dtos.Dashboard
{
    public class MonthWiseTotalResponseDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Total { get; set; }

        public MonthWiseTotalResponseDto() { }

        public MonthWiseTotalResponseDto(string month, decimal total)
        {
            Month = month;
            Total = total;
        }
    }
}

