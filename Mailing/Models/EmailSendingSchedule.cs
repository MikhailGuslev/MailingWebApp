namespace Mailing.Models;

/// <summary>
/// Расписание активации рассылки email
/// </summary>
public sealed record class EmailSendingSchedule
{
    public int EmailSendingScheduleId { get; init; }
    public EmailSending EmailSending { get; init; } = new();
    public DateTime ActivationTimePoint { get; init; }
    public DateTime DeactivationTimePoint { get; init; }
    public TimeSpan ActivationInterval { get; init; }
}