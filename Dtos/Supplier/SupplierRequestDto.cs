using Microsoft.AspNetCore.Http;

namespace BusinessSolution.Dtos.Supplier
{
    public class SupplierRequestDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShopName { get; set; } = string.Empty;
        public string? NidNo { get; set; }
        public string? Email { get; set; }
        public string Mobile { get; set; } = string.Empty;
        public string AdditionalDetails { get; set; } = string.Empty;
        public string PresentAddress { get; set; } = string.Empty;
        public string PermanentAddress { get; set; } = string.Empty;
        public decimal CurrentBalance { get; set; }
        public IFormFile? Image { get; set; }
        public IFormFile? NidImage { get; set; }
        public IFormFile? ChequeImage { get; set; }
    }
}
