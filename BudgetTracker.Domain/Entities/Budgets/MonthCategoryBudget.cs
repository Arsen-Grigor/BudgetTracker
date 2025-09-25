using BudgetTracker.Domain.Entities.Records;
using BudgetTracker.Domain.Exceptions;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Domain.Entities.Budgets;

public class MonthCategoryBudget : Entity
{
    public MonthCategoryBudgetId Id { get; private set; }
    public string UserId { get; private set; }
    public YearMonth Month { get; private set; }
    public RecordSubcategory SubCategory { get; private set; }
    public Money PlannedBudget { get; private set; }
    public Money ActualSpent { get; private set; }
    public Money RemainingBudget => PlannedBudget - ActualSpent;

    public decimal BudgetUtilizationPercentage =>
        PlannedBudget.IsZero ? 0 : (ActualSpent.Amount / PlannedBudget.Amount) * 100;

    public bool IsOverBudget => ActualSpent.Amount > PlannedBudget.Amount;

    private MonthCategoryBudget() { }

    public static MonthCategoryBudget Create(
        MonthCategoryBudgetId id,
        string userId,
        YearMonth month,
        RecordSubcategory subCategory,
        Money plannedBudget,
        Money initialActualSpent = null)
    {
        return new MonthCategoryBudget(id, userId, month, subCategory, plannedBudget,
            initialActualSpent ?? Money.Zero(plannedBudget.Currency));
    }

    private MonthCategoryBudget(
        MonthCategoryBudgetId id,
        string userId,
        YearMonth month,
        RecordSubcategory subCategory,
        Money plannedBudget,
        Money actualSpent)
    {
        ValidateNotNull(id, nameof(id));
        ValidateNotEmpty(userId, nameof(userId));
        ValidateNotNull(month, nameof(month));
        ValidateNotNull(subCategory, nameof(subCategory));
        ValidateNotNull(plannedBudget, nameof(plannedBudget));
        ValidateNotNull(actualSpent, nameof(actualSpent));

        if (plannedBudget.Amount < 0)
            throw new DomainException("Planned budget cannot be negative", "NEGATIVE_PLANNED_BUDGET");

        if (plannedBudget.Currency != actualSpent.Currency)
            throw new DomainException("Planned budget and actual spent must have same currency", "CURRENCY_MISMATCH");

        Id = id;
        UserId = userId;
        Month = month;
        SubCategory = subCategory;
        PlannedBudget = plannedBudget;
        ActualSpent = actualSpent;
    }

    public void ChangePlannedBudget(Money newPlannedBudget)
    {
        ValidateNotNull(newPlannedBudget, nameof(newPlannedBudget));

        if (newPlannedBudget.Amount < 0)
            throw new DomainException("Planned budget cannot be negative", "NEGATIVE_PLANNED_BUDGET");

        if (newPlannedBudget.Currency != PlannedBudget.Currency)
            throw new DomainException("Cannot change currency of existing budget", "CURRENCY_CHANGE_NOT_ALLOWED");

        PlannedBudget = newPlannedBudget;
    }

    public void RegisterRecordExpense(Money expenseAmount)
    {
        ValidateNotNull(expenseAmount, nameof(expenseAmount));

        if (expenseAmount.Currency != ActualSpent.Currency)
            throw new DomainException("Expense currency must match budget currency", "CURRENCY_MISMATCH");

        if (expenseAmount.Amount < 0)
            throw new DomainException("Expense amount cannot be negative", "NEGATIVE_EXPENSE_AMOUNT");

        ActualSpent = ActualSpent + expenseAmount;
    }

    public void AdjustActualSpent(Money newActualSpent)
    {
        ValidateNotNull(newActualSpent, nameof(newActualSpent));

        if (newActualSpent.Currency != ActualSpent.Currency)
            throw new DomainException("Currency cannot be changed", "CURRENCY_CHANGE_NOT_ALLOWED");

        if (newActualSpent.Amount < 0)
            throw new DomainException("Actual spent cannot be negative", "NEGATIVE_ACTUAL_SPENT");

        ActualSpent = newActualSpent;
    }
}