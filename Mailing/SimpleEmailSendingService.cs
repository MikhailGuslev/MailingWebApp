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
        IReadOnlyList<Recipient> recipients,
        MessageContentType messageContentType,
        Message message,
        CancellationToken? cancellationToken = null)
    {
        CancellationToken token = cancellationToken ?? CancellationToken.None;

        using MailKitSmtp.SmtpClient smtpClient = new();
        await smtpClient.ConnectAsync(Settings.SenderServer, Settings.SenderServerPort, false, token);
        await smtpClient.AuthenticateAsync(Settings.SenderEmail, Settings.SenderEmailPassword, token);

        MailboxAddress senderAddress = new MailboxAddress(Settings.SenderServer, Settings.SenderEmail);

        foreach (Recipient recipient in recipients)
        {
            MailboxAddress recipientAddress = new(string.Empty, recipient.Email);

            MimeMessage emailMessage = new();
            emailMessage.From.Add(senderAddress);
            emailMessage.To.Add(recipientAddress);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = CreateMessageBody(messageContentType, message);

            await smtpClient.SendAsync(emailMessage, token);

            string info = "Клиенту с id {userId} отправлено сообщение на тему {subject}";
            Logger.LogInformation(info, recipient.UserId, message.Subject);

            await Task.Yield();
        }

        await smtpClient.DisconnectAsync(true);
    }

    private MimeEntity CreateMessageBody(MessageContentType messageContentType, Message message)
    {
        BodyBuilder bodyBuilder = new();

        switch (messageContentType)
        {
            case MessageContentType.PlainText:
                bodyBuilder.TextBody = message.Body;
                AttacheItems();
                break;
            case MessageContentType.Html:
                bodyBuilder.HtmlBody = message.Body;
                AttacheItems(asLinked: true);
                break;
            default:
                string error =
                    "Невозможно создать email сообщение. " +
                    "Задан неподдерживаемый тип контенат тела сообщения.";
                throw new MailingException(error);
        }

        return bodyBuilder.ToMessageBody();

        void AttacheItems(bool asLinked = false)
        {
            IEnumerable<MimeEntity> attachments = message.Attachments
                .Select(x => x.MapToMimeEntity());
            foreach (MimeEntity item in attachments)
            {
                if (asLinked)
                {
                    bodyBuilder.LinkedResources.Add(item);
                }
                else
                {
                    bodyBuilder.Attachments.Add(item);
                }
            }
        }
    }
}
