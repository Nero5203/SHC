namespace API.Dto.Auth
{
    public class LoginResponseDto
    {
        public string AcessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}