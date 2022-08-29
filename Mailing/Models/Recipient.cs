namespace Mailing.Models;

public sealed record class Recipient
{
    public long UserId { get; init; }
    public string Email { get; init; } = string.Empty;
}
