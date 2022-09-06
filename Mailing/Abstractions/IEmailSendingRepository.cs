using Mailing.Models;

namespace Mailing.Abstractions;

public interface IEmailSendingRepository
{
    Task<IReadOnlyList<EmailSending>> GetEmailSendingsAsync();
    Task<EmailSending> GetEmailSendingByIdAsync(int id);
    Task AddEmailSendingAsync(EmailSending emailSending);
    Task<IReadOnlyList<EmailSendingSchedule>> GetEmailSendingSchedulesAsync();
    Task AddEmailSendingScheduleAsync(EmailSendingSchedule schedule);
}
