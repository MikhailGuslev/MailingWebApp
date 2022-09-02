namespace Mailing.Models;

public sealed record class MessageAttachment
{
    public string MessageAttachmentId { get; init; } = string.Empty;
    public string OriginalContentName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public byte[] Content { get; init; } = Array.Empty<byte>();
}
