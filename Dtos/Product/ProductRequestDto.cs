namespace BusinessSolution.Dtos.Product
{
    public class ProductRequestDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Details { get; set; }
        public IFormFile? Image { get; set; }
    }
}
