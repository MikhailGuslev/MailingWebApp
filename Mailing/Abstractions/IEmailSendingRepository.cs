using Mailing.Models;

namespace Mailing.Abstractions;

public interface IEmailSendingRepository
{
    Task<IReadOnlyList<EmailSending>> GetSendingsAsync();
    Task<IReadOnlyList<EmailSendingSchedule>> GetEmailSendingSchedulesAsync();
    Task AddEmailSendingScheduleAsync(EmailSendingSchedule schedule);
}
