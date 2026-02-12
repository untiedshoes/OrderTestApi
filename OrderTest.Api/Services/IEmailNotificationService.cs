namespace OrderTest.Api.Services;

public interface IEmailNotificationService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendSmsAsync(string phoneNumber, string message);
}