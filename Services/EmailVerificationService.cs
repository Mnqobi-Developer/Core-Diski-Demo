namespace Core_Diski_Demo.Services;

public class EmailVerificationService(ILogger<EmailVerificationService> logger) : IEmailVerificationService
{
    public Task SendVerificationEmailAsync(string toEmail, string confirmationLink)
    {
        logger.LogInformation("Verification email to {Email}. Confirmation link: {Link}", toEmail, confirmationLink);
        return Task.CompletedTask;
    }
}
