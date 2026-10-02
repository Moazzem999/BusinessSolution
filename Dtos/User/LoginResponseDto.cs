namespace BusinessSolution.Dtos.User
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public UserLoginResponseDto User { get; set; } = new UserLoginResponseDto();
    }
}
