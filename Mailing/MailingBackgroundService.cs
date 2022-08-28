using Mailing.Abstractions;
using Mailing.Models;
using Mailing.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mailing;

// TODO: уточнить необходимость в RecipientsProvider
//      - поставщике получателей сообщений. При усложнении системы (push уведомления и т.д.)
//      может потребоваться сложная логика выборки получателей сообщений

// TODO: уточнить необходимость в MessageAttachment
//public sealed record class MessageAttachment
//{
//    public string Name { get; init; }
//    public byte[] Data { get; set; }
//}

/// <summary>
/// Фоновая служба запускающая цикл опроса триггеров рассылок email сообщений
/// </summary>
public sealed class MailingBackgroundService : BackgroundService, IMailingService
{
    private readonly ILogger Logger;
    private readonly IEmailSendingRepository EmailSendingRepository;
    private readonly MailingServiceSettings Settings;

    public MailingBackgroundService(
        ILogger<MailingBackgroundService> logger,
        IEmailSendingRepository sendingRepository,
        IOptions<MailingServiceSettings> settings)
    {
        Logger = logger;
        EmailSendingRepository = sendingRepository;
        Settings = settings.Value;

        EmailSendingScheduler = new(
            Logger,
            EmailSendingRepository,
            Settings);
    }

    /// <summary>
    /// Планировщик рассылок - единственная точка управления рассылками
    /// </summary>
    public EmailSendingScheduler EmailSendingScheduler { get; init; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Logger.LogInformation("MailingService running");

        await EmailSendingScheduler.RestoringStoragedSendingSchedules();
        await RunUpdatingTriggersAsync(stoppingToken);
    }

    private async Task RunUpdatingTriggersAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested is false)
        {
            await UpdateTriggersAsync(stoppingToken);

            // NOTE: важно вызвать Yield() для предотвращения закливания
            await Task.Yield();
        }
    }

    private async Task UpdateTriggersAsync(CancellationToken stoppingToken)
    {
        List<Task> tasks = new(EmailSendingScheduler.ListenedTriggers.Count);

        foreach (EmailSenderTrigger trigger in EmailSendingScheduler.ListenedTriggers)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                return;
            }

            tasks.Add(trigger.UpdateAsync(stoppingToken));
        }

        try
        {
            await Task.WhenAll(tasks.ToArray());
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex);
        }
    }

}
