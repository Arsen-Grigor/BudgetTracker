namespace BudgetTracker.Application.Commands;

public sealed record UpdateRecordCommand(string RecordId, string UserId, decimal? Amount = null, string Description = null);