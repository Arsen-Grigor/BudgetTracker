namespace BudgetTracker.Application.Commands;

public sealed record CreateRecordCommand(string UserId, decimal Amount, int CategoryId, int SubcategoryId, 
    DateTime DateTime, string Description, string Currency = "USD");