using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.Customer
{
    public class CustomerResponseDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string ShopName { get; set; } = string.Empty;
        public string? NidNo { get; set; }
        public string? Email { get; set; }
        public string Mobile { get; set; } = string.Empty;
        public bool IsSpecial { get; set; }
        public string? AdditionalDetails { get; set; }
        public string? PresentAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public decimal CurrentBalance { get; set; }
        public string? ImagePath { get; set; }
        public string? NidImagePath { get; set; }
        public string? ChequeImagePath { get; set; }
    }
}

