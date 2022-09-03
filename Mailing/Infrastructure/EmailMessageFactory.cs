using Mailing.Enums;
using Mailing.Models;
using MimeKit;

namespace Mailing.Infrastructure;

/// <summary>
/// Фабрика экземпляров MimeMessage - объектов-сообщений для smtp клиента 
/// </summary>
public sealed class EmailMessageFactory
{
    private readonly MailboxAddress SenderAddress;

    public EmailMessageFactory(string senderName, string senderEmail, MessageTemplate template)
    {
        SenderName = senderName;
        SenderEmail = senderEmail;

        // NOTE: подготовить экземпдяр фабрики сообщений на основе шаблона
        MessageFactory = new(template);
        SenderAddress = new MailboxAddress(SenderName, SenderEmail);
    }

    public string SenderName { get; init; }
    public string SenderEmail { get; init; }
    public MessageFactory MessageFactory { get; init; }

    public async Task<MimeMessage> CreateEmailMessageAsync(Recipient recipient)
    {
        Message message = await MessageFactory.CreateMessageAsync(recipient);
        MessageContentType messageContentType = MessageFactory.MessageTemplate.ContentType;

        MailboxAddress recipientAddress = new(string.Empty, recipient.Email);

        MimeMessage emailMessage = new();
        emailMessage.From.Add(SenderAddress);
        emailMessage.To.Add(recipientAddress);
        emailMessage.Subject = message.Subject;
        emailMessage.Body = message.MapToMimeEntity(messageContentType);

        return emailMessage;
    }
}
