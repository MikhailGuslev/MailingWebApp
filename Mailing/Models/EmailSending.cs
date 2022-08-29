using DataLayer;

namespace Mailing.Models;

/// <summary>
/// Данные рассылки
/// </summary>
public sealed record class EmailSending
{
    public int SendingId { get; init; }
    public string Name { get; init; } = string.Empty;
    public IReadOnlyList<User> Recipients { get; init; } = new List<User>();
    public MessageTemplate MessageTemplate { get; init; } = new();
}
