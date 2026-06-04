namespace application.Ports.Driving.Auth
{
    public interface IRegisterUserUseCase
    {
        Task Execute(
            string email,
            string password,
            string username,
            string firstName,
            string lastName,
            string phoneNumber);
    }
}
