namespace BusinessSolution.Dtos.Customer
{
    public class CustomerRequestDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShopName { get; set; } = string.Empty;
        public string? NidNo { get; set; }
        public string? Email { get; set; }
        public string Mobile { get; set; } = string.Empty;
        public bool IsSpecial { get; set; } = false;
        public string? AdditionalDetails { get; set; }
        public string? PresentAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public decimal CurrentBalance { get; set; }
        public IFormFile? Image { get; set; }
        public IFormFile? NidImage { get; set; }
        public IFormFile? ChequeImage { get; set; }
    }
}

