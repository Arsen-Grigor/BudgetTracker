using FluentValidation.Results;

namespace BudgetTracker.Application.Exceptions;

public class BadRequestException : Exception  
{
    public BadRequestException(string message) : base(message) { }
    
    public BadRequestException(string message, ValidationResult validationResult) : base(message)
    {
        ValidationErrors = validationResult.ToDictionary();
    }
    
    public IDictionary<string, string[]> ValidationErrors { get; set; } = new Dictionary<string, string[]>();
}