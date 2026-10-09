using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.Supplier
{
    public class SupplierResponseDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string ShopName { get; set; } = string.Empty;
        public string NidNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string AdditionalDetails { get; set; } = string.Empty;
        public string PresentAddress { get; set; } = string.Empty;
        public string PermanentAddress { get; set; } = string.Empty;
        public decimal CurrentBalance { get; set; }
        public string? ImagePath { get; set; }
        public string? NidImagePath { get; set; }
        public string? ChequeImagePath { get; set; }
    }
}
