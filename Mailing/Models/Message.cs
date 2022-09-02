using MimeKit;

namespace Mailing.Models;

public sealed record Message
{
    public string Subject { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public IReadOnlyList<MimeEntity> Attachments { get; init; } = new List<MimeEntity>();
}