using BudgetTracker.Application.Commands;
using FluentValidation;

namespace BudgetTracker.Application.Validations;

public class UpdateRecordValidator : AbstractValidator<UpdateRecordCommand>
{
    public UpdateRecordValidator()
    {
        RuleFor(x => x.RecordId).NotEmpty().Must(BeValidGuid);
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Amount).NotEqual(0).When(x => x.Amount.HasValue);
        RuleFor(x => x.Description).MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Description));
    }
    
    private bool BeValidGuid(string guid) => Guid.TryParse(guid, out _);
}