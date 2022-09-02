using Mailing.Enums;

namespace Mailing.Models;

public sealed record class RecurrenceMonth
{
    public bool EveryMonth { get; init; } = false;
    public Month? ConcreteMonth { get; init; } = null;
    public bool IsEmpty => EveryMonth is false && ConcreteMonth is null;
    public static RecurrenceMonth Empty => new();
}
