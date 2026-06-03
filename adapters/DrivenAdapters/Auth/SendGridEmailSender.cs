namespace adapters.DrivenAdapters.Auth
{
    using ports.DrivenPorts.Auth;
    using SendGrid;
    using SendGrid.Helpers.Mail;
    public class SendGridEmailSender : IEmailSender
    {
        private readonly string _ApiKey;

        public SendGridEmailSender(string apiKey)
        {
            _ApiKey = apiKey;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var client = new SendGridClient(_ApiKey);
            var from = new EmailAddress("qunileke@gmail.com", "SHC Support");
            var recipient = new EmailAddress(to);

            var msg = MailHelper.CreateSingleEmail(from, recipient, subject,plainTextContent: null, htmlContent: body);
            var response = await client.SendEmailAsync(msg);

            if(!response.IsSuccessStatusCode)
            {
                // Log the error or throw an exception
                throw new Exception($"Failed to send email: {response.StatusCode}");
            }
        }
    }
}