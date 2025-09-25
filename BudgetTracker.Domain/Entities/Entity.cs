using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Domain.Entities;

public abstract class Entity
{
    protected void ValidateNotNull(object value, string paramName)
    {
        if (value is null)
            throw new DomainException($"{paramName} cannot be null", "NULL_VALUE");
    }

    protected void ValidateNotEmpty(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{paramName} cannot be null or empty", "EMPTY_VALUE");
    }
}