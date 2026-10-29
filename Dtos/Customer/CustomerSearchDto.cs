using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.Customer
{
    public class CustomerSearchDto : PaginationDto
    {
        public string SearchTerm { get; set; } = string.Empty;
        public bool? IsSpecial { get; set; }
    }
}

