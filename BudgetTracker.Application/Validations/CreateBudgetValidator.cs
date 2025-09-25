using BudgetTracker.Application.Commands;
using FluentValidation;

namespace BudgetTracker.Application.Validations;

public class CreateBudgetValidator : AbstractValidator<CreateBudgetCommand>
{
    public CreateBudgetValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Year).InclusiveBetween(1900, 3000);
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
        RuleFor(x => x.SubcategoryId).GreaterThan(0);
        RuleFor(x => x.PlannedAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Currency).Length(3);
    }
}