namespace Core_Diski_Demo.Services;

public interface IEmailVerificationService
{
    Task SendVerificationEmailAsync(string toEmail, string confirmationLink);
}
