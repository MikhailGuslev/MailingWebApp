using Mailing.Infrastructure;

namespace Mailing.Models;

/// <summary>
/// Настройка повторений по дням месяца или по дням недели
/// </summary>
public sealed record class Recurrence
{
    public TimeOnly Time { get; init; } = TimeOnly.MinValue;
    public RecurrenceDayOfWeek DayOfWeek { get; init; } = RecurrenceDayOfWeek.Empty;
    public RecurrenceDayOfMonth DayOfMonth { get; init; } = RecurrenceDayOfMonth.Empty;
    public RecurrenceMonth Month { get; init; } = RecurrenceMonth.Empty;
    public bool IsEmpty => DayOfWeek.IsEmpty && Month.IsEmpty && DayOfMonth.IsEmpty;

    public static Recurrence Empty => new();

    public DateTime GetNextFiring(DateTime originalPoint)
    {
        Validate();

        if (DayOfWeek.IsEmpty is false)
        {
            return GetNextFiringByWeekSettings(originalPoint);
        }

        if (DayOfMonth.IsEmpty is false)
        {
            return GetNextFiringByMonthSettings(originalPoint);
        }

        string error = "Не удалось рассчитать следующую точку активации";
        throw new MailingException(error);
    }

    public void Validate()
    {
        if (Month.IsEmpty is false && DayOfWeek.IsEmpty is false)
        {
            string error =
                "Неверно настроено расписание повторений - " +
                "присутствуют настройки повторения относительно дней месяца " +
                "и повторение относительно дней недели";
            throw new MailingException(error);
        }

        if (Month.IsEmpty is false && DayOfMonth.IsEmpty)
        {
            string error = "Неверно настроено расписание повторений - задан месяц, но не указан день активации";
            throw new MailingException(error);
        }

        if (Month.IsEmpty is false && Month.ConcreteMonth != null && DayOfMonth.ConcreteDay != null)
        {
            DateTime now = DateTime.Now;
            int lastMonthDay = DateTime.DaysInMonth(now.Year, (int)Month.ConcreteMonth);
            if (lastMonthDay < DayOfMonth.ConcreteDay || DayOfMonth.ConcreteDay < 1)
            {
                string error =
                    "Неверно настроено расписание повторений - " +
                    "задан день выходящий за диапазон допустимых дней заданного месяца";
                throw new MailingException(error);
            }
        }

        if (DayOfWeek.IsEmpty is false
            && DayOfWeek.ConcreteDay != null
            && (DayOfWeek.ConcreteDay < System.DayOfWeek.Sunday
            || DayOfWeek.ConcreteDay > System.DayOfWeek.Saturday))
        {
            string error =
                    "Неверно настроено расписание повторений - " +
                    "задан невалидный номер деня недели";
            throw new MailingException(error);
        }
    }

    private DateTime GetNextFiringByMonthSettings(DateTime originalPoint)
    {
        DateTime tmp = originalPoint;

        if (tmp.TimeOfDay > Time.ToTimeSpan())
        {
            tmp = tmp.AddDays(1);
        }

        tmp = new DateTime(tmp.Year, tmp.Month, tmp.Day, Time.Hour, Time.Minute, Time.Second);

        if (DayOfMonth.ConcreteDay != null)
        {
            int targetDay = DayOfMonth.ConcreteDay.Value;
            int diff = targetDay - tmp.Day;

            tmp = tmp.AddDays(diff);
            if (diff < 0)
            {
                tmp = tmp.AddMonths(1);
            }
        }

        if (Month.ConcreteMonth != null)
        {
            int targetMonth = (int)Month.ConcreteMonth;
            int diff = targetMonth - tmp.Month;
            tmp = tmp.AddMonths(diff);

            if (diff != 0 && DayOfMonth.Special == RecurrenceDayOfMonthSpecial.EveryDay)
            {
                tmp = tmp.AddDays(-(tmp.Day - 1));
            }

            if (diff < 0)
            {
                tmp = tmp.AddYears(1);
            }
        }

        if (DayOfMonth.Special == RecurrenceDayOfMonthSpecial.LastDey)
        {
            int lastday = DateTime.DaysInMonth(tmp.Year, tmp.Month);
            tmp = tmp.AddDays(lastday - tmp.Day);
        }

        return tmp;
    }

    private DateTime GetNextFiringByWeekSettings(DateTime originalPoint)
    {
        DateTime tmp = originalPoint;

        if (tmp.TimeOfDay > Time.ToTimeSpan())
        {
            tmp = tmp.AddDays(1);
        }

        tmp = new DateTime(tmp.Year, tmp.Month, tmp.Day, Time.Hour, Time.Minute, Time.Second);

        if (DayOfWeek.EveryDay)
        {
            return tmp;
        }

        if (DayOfWeek.ConcreteDay != null)
        {
            DayOfWeek targetDay = DayOfWeek.ConcreteDay.Value;
            DayOfWeek originalDay = originalPoint.DayOfWeek;

            int diff = targetDay - originalDay;
            int addDays = diff < 0
                ? Math.Abs(diff)
                : 7 - diff;

            tmp = tmp.AddDays(addDays);
        }

        return tmp;
    }
}