using Mailing.Models;

namespace Mailing.Abstractions;

public interface IEmailSendingRepository
{
    IReadOnlyList<EmailSending> GetSendings();
    IReadOnlyList<EmailSendingSchedule> GetEmailSendingSchedules();
    void AddEmailSendingSchedule(EmailSendingSchedule schedule);
}
