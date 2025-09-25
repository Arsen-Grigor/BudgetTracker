namespace BudgetTracker.Application.Queries;

public sealed record GetRecordsQuery(string UserId, int Year, int Month, int? SubcategoryId = null);
