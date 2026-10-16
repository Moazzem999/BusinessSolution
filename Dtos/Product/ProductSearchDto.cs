using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.Product
{
    public class ProductSearchDto : PaginationDto
    {
        public string SearchTerm { get; set; } = string.Empty;
    }
}
