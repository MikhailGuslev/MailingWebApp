namespace Mailing.Models;

/// <summary>
/// Данные рассылки
/// </summary>
public sealed record class EmailSending
{
    public int EmailSendingId { get; init; }
    public string Name { get; init; } = string.Empty;
    public IReadOnlyList<Recipient> Recipients { get; init; } = new List<Recipient>();
    public MessageTemplate MessageTemplate { get; init; } = new();
}
