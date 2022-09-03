using Mailing.Enums;
using Mailing.Models;
using MimeKit;

namespace Mailing.Infrastructure;

public static class MessageExtensions
{
    public static MimeEntity MapToMimeEntity(this Message message, MessageContentType messageContentType)
    {
        BodyBuilder bodyBuilder = new();

        switch (messageContentType)
        {
            case MessageContentType.PlainText:
                bodyBuilder.TextBody = message.Body;
                AttacheItems(message, bodyBuilder);
                break;
            case MessageContentType.Html:
                bodyBuilder.HtmlBody = message.Body;
                AttacheItems(message, bodyBuilder, asLinked: true);
                break;
            default:
                string error =
                    "Невозможно создать email сообщение. " +
                    "Задан неподдерживаемый тип контенат тела сообщения.";
                throw new MailingException(error);
        }

        return bodyBuilder.ToMessageBody();
    }

    private static void AttacheItems(Message message, BodyBuilder bodyBuilder, bool asLinked = false)
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
