namespace BudgetTracker.Application.Commands;

public sealed record CreateBudgetCommand(string UserId, int Year, int Month, int SubcategoryId, 
    decimal PlannedAmount, string Currency = "USD");