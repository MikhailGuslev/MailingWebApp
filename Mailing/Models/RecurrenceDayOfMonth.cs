namespace Mailing.Models;

public enum RecurrenceDayOfMonthSpecial
{
    EveryDay = 1,
    LastDey
}

public sealed record class RecurrenceDayOfMonth
{
    public RecurrenceDayOfMonthSpecial? Special { get; init; } = null;
    public int? ConcreteDay { get; init; } = null;
    public bool IsEmpty => Special is null && ConcreteDay is null;
    public static RecurrenceDayOfMonth Empty => new();
}
