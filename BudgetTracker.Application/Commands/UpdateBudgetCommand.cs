namespace BudgetTracker.Application.Commands;

public sealed record UpdateBudgetCommand(string BudgetId, string UserId, decimal PlannedAmount);