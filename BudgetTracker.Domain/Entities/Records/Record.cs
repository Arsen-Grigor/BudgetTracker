using BudgetTracker.Domain.Exceptions;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Domain.Entities.Records;

public class Record : Entity
{
    public RecordId Id { get; private set; }
    public string UserId { get; private set; } 
    public Money Amount { get; private set; }
    public RecordCategory Category { get; private set; }
    public RecordSubcategory Subcategory { get; private set; }
    public DateTime Time { get; private set; }
    public Note Description { get; private set; }

    private Record() { }

    public static Record Create(
        RecordId id,
        string userId,
        Money amount,
        RecordCategory category,
        RecordSubcategory subcategory,
        DateTime dateTime,
        Note description)
    {
        return new Record(id, userId, amount, category, subcategory, dateTime, description);
    }

    private Record(
        RecordId id,
        string userId,
        Money amount,
        RecordCategory category,
        RecordSubcategory subcategory,
        DateTime time,
        Note description)
    {
        ValidateNotNull(id, nameof(id));
        ValidateNotEmpty(userId, nameof(userId));
        ValidateNotNull(amount, nameof(amount));
        ValidateNotNull(category, nameof(category));
        ValidateNotNull(subcategory, nameof(subcategory));
        ValidateNotNull(description, nameof(description));

        if (!category.HasSubcategory(subcategory.Id))
            throw new DomainException($"Subcategory {subcategory.Id} does not belong to category {category.Id}",
                "INVALID_SUBCATEGORY_CATEGORY");

        Id = id;
        UserId = userId; // 9.25 - that was issue related to multi-access.
        Amount = amount;
        Category = category;
        Subcategory = subcategory;
        Time = time;
        Description = description;
    }

    public void UpdateAmount(Money newAmount)
    {
        ValidateNotNull(newAmount, nameof(newAmount));

        if (newAmount.Currency != Amount.Currency)
            throw new DomainException("Cannot change currency of existing record", "CURRENCY_CHANGE_NOT_ALLOWED");

        Amount = newAmount;
    }

    public void UpdateDescription(Note newDescription)
    {
        ValidateNotNull(newDescription, nameof(newDescription));

        Description = newDescription;
    }
}