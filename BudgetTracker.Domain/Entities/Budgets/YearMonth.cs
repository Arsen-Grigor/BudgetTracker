using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Domain.Entities.Budgets;

public sealed record YearMonth
{
    public int Year { get; }
    public int Month { get; }

    public YearMonth(int year, int month)
    {
        if (year < 1900 || year > 3000)
            throw new DomainException($"Year must be between 1900 and 3000, got: {year}", "INVALID_YEAR");

        if (month < 1 || month > 12)
            throw new DomainException($"Month must be between 1 and 12, got: {month}", "INVALID_MONTH");

        Year = year;
        Month = month;
    }

    public static YearMonth Current => new(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
    public static YearMonth FromDateTime(DateTime dateTime) => new(dateTime.Year, dateTime.Month);
    public DateTime FirstDayOfMonth => new(Year, Month, 1);
    public DateTime LastDayOfMonth => FirstDayOfMonth.AddMonths(1).AddDays(-1);

    public YearMonth NextMonth()
    {
        if (Month == 12)
            return new YearMonth(Year + 1, 1);
        return new YearMonth(Year, Month + 1);
    }

    public YearMonth PreviousMonth()
    {
        if (Month == 1)
            return new YearMonth(Year - 1, 12);
        return new YearMonth(Year, Month - 1);
    }

    public override string ToString() => $"{Year:D4}-{Month:D2}";
}