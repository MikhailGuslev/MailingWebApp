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

        MailboxAddress recipientAddress = new(string.Empty, recipient.Email);

        MimeMessage emailMessage = new();
        emailMessage.From.Add(SenderAddress);
        emailMessage.To.Add(recipientAddress);
        emailMessage.Subject = message.Subject;
        emailMessage.Body = CreateMessageBody(message);

        return emailMessage;
    }

    private MimeEntity CreateMessageBody(Message message)
    {
        MessageContentType contentType = MessageFactory.MessageTemplate.ContentType;

        BodyBuilder bodyBuilder = new();

        switch (contentType)
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
