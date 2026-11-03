namespace BusinessSolution.Dtos.Dashboard
{
    public class DayWiseTotalResponseDto
    {
        public string Day { get; set; } = string.Empty;
        public decimal Total { get; set; }

        public DayWiseTotalResponseDto() { }

        public DayWiseTotalResponseDto(string day, decimal total)
        {
            Day = day;
            Total = total;
        }
    }
}

