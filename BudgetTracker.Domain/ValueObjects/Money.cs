using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency = "USD")
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency cannot be null or empty", "INVALID_CURRENCY");

        if (currency.Length != 3)
            throw new DomainException("Currency must be 3 characters long", "INVALID_CURRENCY_FORMAT");

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static Money Zero(string currency = "USD") => new(0, currency);
    public static Money Create(decimal amount, string currency = "USD") => new(amount, currency);

    public static Money operator +(Money left, Money right)
    {
        ValidateSameCurrency(left, right);
        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        ValidateSameCurrency(left, right);
        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        return new Money(money.Amount * multiplier, money.Currency);
    }

    public Money Abs() => new(Math.Abs(Amount), Currency);
    public bool IsPositive => Amount > 0;
    public bool IsNegative => Amount < 0;
    public bool IsZero => Amount == 0;

    private static void ValidateSameCurrency(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new DomainException(
                $"Cannot perform operation on different currencies: {left.Currency} and {right.Currency}",
                "CURRENCY_MISMATCH");
    }

    public override string ToString() => $"{Amount:F2} {Currency}";
}