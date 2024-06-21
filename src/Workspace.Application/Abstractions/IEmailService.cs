namespace Workspace.Application
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail);
        Task<bool> VerifyEmailAsync(string email, string otp);
        Task SendEmailV2Async(string toEmail, Guid id, string action);
        Task SendEmailResetPasswordAsync(string email, string password);
        Task SendEmailReminderAsync();
    }
}
