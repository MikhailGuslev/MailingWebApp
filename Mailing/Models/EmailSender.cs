using DataLayer;
using Mailing.Infrastructure;
using Mailing.Settings;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Text;
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

    public EmailSender(
        ILogger logger,
        MailingServiceSettings settings,
        User[] recipients,
        EmailMessageFactory emailMessageFactory)
    {
        Logger = logger;
        Recipients = recipients;
        EmailMessageFactory = emailMessageFactory;
        Settings = settings;
    }

    public User[] Recipients { get; init; }
    public EmailMessageFactory EmailMessageFactory { get; init; }
    public MailingServiceSettings Settings { get; init; }

    public async Task RunSendingAsync(CancellationToken stoppingToken)
    {
        using MailKitSmtp.SmtpClient client = new();

        await client.ConnectAsync(Settings.SenderServer, Settings.SenderServerPort, false, stoppingToken);
        await client.AuthenticateAsync(Settings.SenderEmail, Settings.SenderEmailPassword, stoppingToken);

        foreach (User recipient in Recipients)
        {
            MimeMessage emailMessage = await EmailMessageFactory.CreateEmailMessageAsync(recipient);
            await client.SendAsync(emailMessage, stoppingToken);
            LogSendingSummary(recipient, emailMessage);
            await Task.Yield();
        }

        await client.DisconnectAsync(true);
    }

    private void LogSendingSummary(User recipient, MimeMessage message)
    {
        string info = new StringBuilder()
            .Append("Пользователю ")
            .Append(recipient.UserId)
            .Append(" отправлено сообщение на тему")
            .Append(message.Subject)
            .ToString();

        Logger.LogInformation(info);
    }
}