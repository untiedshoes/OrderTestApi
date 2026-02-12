namespace OrderTest.Api.Services;


//make all methods async
public class EmailNotificationService : INotificationService
{
    public Task SendEmailAsync(string to, string subject, string body)
    {
        Console.WriteLine($"Sending email to {to}: {subject}");
        return Task.CompletedTask;
    }

    public Task SendSmsAsync(string phoneNumber, string message)
    {
        return Task.FromException(new NotImplementedException());
    }
}
