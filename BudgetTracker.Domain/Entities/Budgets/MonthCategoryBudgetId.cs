namespace BudgetTracker.Domain.Entities.Budgets;

public sealed record MonthCategoryBudgetId(Guid Value)
{
    public static MonthCategoryBudgetId New() => new(Guid.NewGuid());
    public static explicit operator Guid(MonthCategoryBudgetId budgetId) => budgetId.Value;
    public static implicit operator MonthCategoryBudgetId(Guid value) => new(value);
}