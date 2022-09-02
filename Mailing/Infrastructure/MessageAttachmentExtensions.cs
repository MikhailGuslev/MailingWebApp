using Mailing.Models;
using MimeKit;

namespace Mailing.Infrastructure;

public static class MessageAttachmentExtensions
{
    public static MimeEntity MapToMimeEntity(this MessageAttachment original)
    {
        MemoryStream contentDataStream = new(original.Content);

        MimePart mimeEntity = new(original.ContentType)
        {
            ContentId = original.MessageAttachmentId,
            Content = new MimeContent(contentDataStream, ContentEncoding.Default),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = Path.GetFileName(original.OriginalContentName)
        };

        return mimeEntity;
    }
}
