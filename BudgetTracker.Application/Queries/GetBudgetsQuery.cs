namespace BudgetTracker.Application.Queries;

public record GetBudgetsQuery(string UserId, int Year, int Month);