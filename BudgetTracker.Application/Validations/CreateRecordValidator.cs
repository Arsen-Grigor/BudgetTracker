using BudgetTracker.Application.Commands;
using FluentValidation;

namespace BudgetTracker.Application.Validations;

public class CreateRecordValidator : AbstractValidator<CreateRecordCommand>
{
    public CreateRecordValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Amount).NotEqual(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.SubcategoryId).GreaterThan(0);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Currency).Length(3);
    }
}