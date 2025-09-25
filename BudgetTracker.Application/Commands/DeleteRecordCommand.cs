namespace BudgetTracker.Application.Commands;

public sealed record DeleteRecordCommand(string RecordId, string UserId);