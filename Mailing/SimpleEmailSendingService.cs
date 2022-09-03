using Mailing.Enums;
using Mailing.Infrastructure;
using Mailing.Models;
using Mailing.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKitSmtp = MailKit.Net.Smtp;

namespace Mailing;

public sealed class SimpleEmailSendingService
{
    private readonly ILogger<SimpleEmailSendingService> Logger;
    private readonly MailingServiceSettings Settings;

    public SimpleEmailSendingService(
        ILogger<SimpleEmailSendingService> logger,
        IOptions<MailingServiceSettings> settings)
    {
        Logger = logger;
        Settings = settings.Value;
    }

    public async Task SendEmailMessageAsync(
        Recipient recipient,
        string subject,
        string messageText,
        CancellationToken? cancellationToken = null)
    {
        Message message = new()
        {
            Subject = subject,
            Body = messageText,
            Attachments = Enumerable
                .Empty<MessageAttachment>()
                .ToList()
        };

        CancellationToken token = cancellationToken ?? CancellationToken.None;
        using MailKitSmtp.SmtpClient smtpClient = await GetSmtpClientAsync(token);
        MailboxAddress senderAddress = new MailboxAddress(Settings.SenderServer, Settings.SenderEmail);
        await SendAsync(
            smtpClient,
            senderAddress,
            recipient,
            MessageContentType.PlainText,
            message,
            token);

        await smtpClient.DisconnectAsync(true);
    }

    public async Task SendEmailMessageAsync(
        IReadOnlyList<Recipient> recipients,
        string subject,
        string messageText,
        CancellationToken? cancellationToken = null)
    {
        Message message = new()
        {
            Subject = subject,
            Body = messageText,
            Attachments = Enumerable
                .Empty<MessageAttachment>()
                .ToList()
        };

        await SendEmailMessageAsync(
            recipients,
            MessageContentType.PlainText,
            message,
            cancellationToken);
    }

    public async Task SendEmailMessageAsync(
        IReadOnlyList<Recipient> recipients,
        MessageContentType messageContentType,
        Message message,
        CancellationToken? cancellationToken = null)
    {
        CancellationToken token = cancellationToken ?? CancellationToken.None;

        using MailKitSmtp.SmtpClient smtpClient = await GetSmtpClientAsync(token);
        MailboxAddress senderAddress = new MailboxAddress(Settings.SenderServer, Settings.SenderEmail);

        foreach (Recipient recipient in recipients)
        {
            await SendAsync(smtpClient, senderAddress, recipient, messageContentType, message, token);
            await Task.Yield();
        }

        await smtpClient.DisconnectAsync(true);
    }

    private async Task<MailKitSmtp.SmtpClient> GetSmtpClientAsync(CancellationToken token)
    {
        MailKitSmtp.SmtpClient smtpClient = new();
        await smtpClient.ConnectAsync(
            Settings.SenderServer,
            Settings.SenderServerPort,
            false,
            token);
        await smtpClient.AuthenticateAsync(
            Settings.SenderEmail,
            Settings.SenderEmailPassword,
            token);

        return smtpClient;
    }

    private async Task SendAsync(
        MailKitSmtp.SmtpClient smtpClient,
        MailboxAddress senderAddress,
        Recipient recipient,
        MessageContentType messageContentType,
        Message message,
        CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return;
        }

        MailboxAddress recipientAddress = new(string.Empty, recipient.Email);

        MimeMessage emailMessage = new();
        emailMessage.From.Add(senderAddress);
        emailMessage.To.Add(recipientAddress);
        emailMessage.Subject = message.Subject;
        emailMessage.Body = message.MapToMimeEntity(messageContentType);

        await smtpClient.SendAsync(emailMessage, token);

        string info = "Клиенту с id {userId} отправлено сообщение на тему {subject}";
        Logger.LogInformation(info, recipient.UserId, message.Subject);
    }
}
