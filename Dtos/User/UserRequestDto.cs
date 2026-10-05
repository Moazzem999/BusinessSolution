namespace BusinessSolution.Dtos.User
{
    public class UserRequestDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Mobile { get; set; } = string.Empty;
    }
}
