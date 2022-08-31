using Mailing.Infrastructure;
using Mailing.Settings;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKitSmtp = MailKit.Net.Smtp;

namespace Mailing.Models;

/// <summary>
/// Отправщик сообщения заданным получателям
/// </summary>
public sealed record class EmailSender
{
    private readonly ILogger Logger;

    // TODO: реализовать механизм отмены для случаев
    //       - сборка поставщиков моделей выгружена
    //       - рассылка отменена

    public EmailSender(ILogger logger, MailingServiceSettings settings, EmailSending sending)
    {
        Logger = logger;
        Settings = settings;
        Sending = sending;

        EmailMessageFactory = new(
            Settings.SenderServer,
            Settings.SenderEmail,
            sending.MessageTemplate);
    }

    public MailingServiceSettings Settings { get; init; }
    public EmailSending Sending { get; init; }
    public EmailMessageFactory EmailMessageFactory { get; init; }

    public async Task RunSendingAsync(CancellationToken stoppingToken)
    {
        Logger.LogInformation("Активация рассылки {sending}", new { Sending.EmailSendingId, Sending.Name });

        using MailKitSmtp.SmtpClient smtpClient = new();
        await smtpClient.ConnectAsync(Settings.SenderServer, Settings.SenderServerPort, false, stoppingToken);
        await smtpClient.AuthenticateAsync(Settings.SenderEmail, Settings.SenderEmailPassword, stoppingToken);

        foreach (Recipient recipient in Sending.Recipients)
        {
            MimeMessage emailMessage = await EmailMessageFactory
                .CreateEmailMessageAsync(recipient);
            await smtpClient.SendAsync(emailMessage, stoppingToken);

            string info = "Клиенту с id {userId} отправлено сообщение на тему {subject}";
            Logger.LogInformation(info, recipient.UserId, emailMessage.Subject);

            await Task.Yield();
        }

        Logger.LogInformation("Рассылки {sending} завершена", new { Sending.EmailSendingId, Sending.Name });

        await smtpClient.DisconnectAsync(true);
    }
}