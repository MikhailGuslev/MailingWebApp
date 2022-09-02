using Mailing.Infrastructure;

namespace Mailing.Models;

/// <summary>
/// Расписание активации рассылки email
/// </summary>
public sealed record class EmailSendingSchedule
{
    public EmailSendingSchedule()
    {
    }

    public EmailSendingSchedule(DateTime lastactivation)
    {
        LastActivation = lastactivation;
    }

    public int EmailSendingScheduleId { get; init; }
    public EmailSending EmailSending { get; init; } = new();
    public DateTime ActivationTimePoint { get; init; } = DateTime.MinValue;
    public DateTime DeactivationTimePoint { get; init; } = DateTime.MinValue;
    public Recurrence RecurrenceActivation { get; init; } = Recurrence.Empty;

    public DateTime LastActivation { get; private set; } = DateTime.MinValue;

    public void Activate()
    {
        LastActivation = DateTime.Now;
    }

    public DateTime GetNextActivation(DateTime? originalPoint = null)
    {
        if ((originalPoint ?? DateTime.Now) >= DeactivationTimePoint)
        {
            return DateTime.MaxValue;
        }

        return RecurrenceActivation.GetNextFiring(originalPoint ?? DateTime.Now);
    }

    public void Validate()
    {
        if (ActivationTimePoint > DeactivationTimePoint)
        {
            string error = "некорректно настроено расписание - дата начала активации превышает дату деактивации";
            throw new MailingException(error);
        }

        if (RecurrenceActivation.IsEmpty is false)
        {
            RecurrenceActivation.Validate();
        }
    }
}