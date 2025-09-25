namespace BudgetTracker.Application.Commands;

public sealed record DeleteBudgetCommand(string BudgetId, string UserId);