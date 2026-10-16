using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.Product
{
    public class ProductResponseDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
    }
}
