namespace application.UseCases.Auth
{
    public class LogoutRequest
    {
        public string RefreshToken { get; set; } = null!;
    }
}