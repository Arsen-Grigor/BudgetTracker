namespace BudgetTracker.Application.DTOs;

public sealed record BudgetDto(string Id, string SubcategoryName, decimal PlannedAmount, decimal ActualSpent, 
    decimal RemainingAmount, decimal UtilizationPercentage, bool IsOverBudget, string Currency);