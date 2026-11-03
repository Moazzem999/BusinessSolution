namespace BusinessSolution.Dtos.Dashboard
{
    public class CustomerCountResponseDto
    {
        public int TotalCount { get; set; }

        public CustomerCountResponseDto() { }

        public CustomerCountResponseDto(int totalCount)
        {
            TotalCount = totalCount;
        }
    }
}

