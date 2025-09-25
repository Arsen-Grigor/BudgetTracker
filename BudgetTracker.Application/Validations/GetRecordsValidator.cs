using BudgetTracker.Application.Queries;
using FluentValidation;

namespace BudgetTracker.Application.Validations;

public class GetRecordsValidator : AbstractValidator<GetRecordsQuery>
{
    public GetRecordsValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Year).InclusiveBetween(1900, 3000);
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
    }
}