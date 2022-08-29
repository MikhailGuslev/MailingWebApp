using DataLayer;
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
        Logger.LogInformation("Запуск рассылки {sending}", new { Sending.SendingId, Sending.Name });

        using MailKitSmtp.SmtpClient smtpClient = new();
        await smtpClient.ConnectAsync(Settings.SenderServer, Settings.SenderServerPort, false, stoppingToken);
        await smtpClient.AuthenticateAsync(Settings.SenderEmail, Settings.SenderEmailPassword, stoppingToken);

        foreach (User recipient in Sending.Recipients)
        {
            MimeMessage emailMessage = await EmailMessageFactory
                .CreateEmailMessageAsync(recipient);
            await smtpClient.SendAsync(emailMessage, stoppingToken);

            LogSendingSummary(recipient, emailMessage);
            await Task.Yield();
        }

        await smtpClient.DisconnectAsync(true);
    }

    private void LogSendingSummary(User recipient, MimeMessage message)
    {
        string info = "Клиенту с id {userId} отправлено сообщение на тему {subject}";
        Logger.LogInformation(info, recipient.UserId, message.Subject);
    }
}