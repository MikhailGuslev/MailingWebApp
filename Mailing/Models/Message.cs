namespace Mailing.Models;

public sealed record Message
{
    public string Subject { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
}