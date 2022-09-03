namespace Mailing.Models;

/// <summary>
/// Триггер, запускающий рассылку в соответствии с расписанием
/// </summary>
internal sealed record class EmailSenderTrigger
{
    internal enum TriggerState { Waiting = 0, Triggered, Deactivated }

    internal TriggerState CurrentState = TriggerState.Waiting;

    private DateTime NextActivation = DateTime.MinValue;

    public EmailSenderTrigger(EmailSender targetSender, EmailSendingSchedule sendingSchedule)
    {
        TargetSender = targetSender;
        Schedule = sendingSchedule;

        CurrentState = GetNextState();
    }

    public EmailSender TargetSender { get; init; }
    public EmailSendingSchedule Schedule { get; init; }

    public async Task UpdateAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested || CurrentState == TriggerState.Deactivated)
        {
            return;
        }

        CurrentState = GetNextState();

        if (CurrentState == TriggerState.Triggered)
        {
            await TargetSender.RunSendingAsync(stoppingToken);
        }
    }

    private TriggerState GetNextState()
    {
        if (NextActivation == DateTime.MinValue)
        {
            NextActivation = Schedule.GetNextActivation(Schedule.LastActivation);
        }

        DateTime now = DateTime.Now;

        if (now >= Schedule.DeactivationTimePoint)
        {
            return TriggerState.Deactivated;
        }

        if (DateTime.Now >= NextActivation)
        {
            NextActivation = Schedule.GetNextActivation(DateTime.Now);
            return TriggerState.Triggered;
        }

        return TriggerState.Waiting;
    }
}
