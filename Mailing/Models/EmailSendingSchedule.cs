namespace Mailing.Models;

/// <summary>
/// Расписание активации рассылки email
/// </summary>
public sealed record class EmailSendingSchedule
{
    public int EmailSendingScheduleId { get; init; }
    public EmailSending EmailSending { get; init; } = new();
    public DateTime ActivationTimePoint { get; init; } = DateTime.MinValue;
    public DateTime DeactivationTimePoint { get; init; } = DateTime.MinValue;
    public TimeSpan ActivationInterval { get; init; } = TimeSpan.Zero;

    public DateTime LastActivation { get; private set; } = DateTime.MinValue;

    public void Activate()
    {
        LastActivation = DateTime.Now;
    }

    public DateTime? GetNextActivation()
    {
        if (DateTime.Now >= DeactivationTimePoint) return null;

        TimeSpan elapsed = DateTime.Now - LastActivation;

        if (elapsed > ActivationInterval)
        {
            // NOTE: актуализация времени последней активации
            (long countActivation, long remainder) = Math
                .DivRem(elapsed.Ticks, ActivationInterval.Ticks);

            LastActivation += countActivation * ActivationInterval;
            elapsed = DateTime.Now - LastActivation;
        }

        DateTime maybeNext = LastActivation + ActivationInterval - elapsed;

        return maybeNext >= DeactivationTimePoint
            ? null
            : maybeNext;
    }

    public bool Validate()
    {
        return ActivationTimePoint < DeactivationTimePoint
            && (ActivationTimePoint + ActivationInterval) < DeactivationTimePoint;
    }
}