using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mailing.Models;

public sealed record class RecurrenceDayOfWeek
{
    public bool EveryDay { get; init; } = false;
    public DayOfWeek? ConcreteDay { get; init; } = null;
    public bool IsEmpty => EveryDay is false && ConcreteDay is null;
    public static RecurrenceDayOfWeek Empty => new();
}
