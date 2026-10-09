using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.Supplier
{
    public class SupplierSearchDto : PaginationDto
    {
        public string SearchTerm { get; set; } = string.Empty;
    }
}
