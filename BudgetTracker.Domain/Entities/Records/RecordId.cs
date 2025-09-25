namespace BudgetTracker.Domain.Entities.Records;

public sealed record RecordId(Guid Value)
{
    public static RecordId New() => new(Guid.NewGuid());
    public static explicit operator Guid(RecordId recordId) => recordId.Value;
    public static implicit operator RecordId(Guid value) => new(value);
}