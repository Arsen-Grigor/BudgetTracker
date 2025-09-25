namespace BudgetTracker.Application.DTOs;

public sealed record RecordDto(string Id, decimal Amount, string Currency, string CategoryName, 
    string SubcategoryName, DateTime DateTime, string Description);