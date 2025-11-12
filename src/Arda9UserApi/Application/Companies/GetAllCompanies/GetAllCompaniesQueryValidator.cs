using FluentValidation;

namespace Arda9UserApi.Application.Companies.GetAllCompanies;

public class GetAllCompaniesQueryValidator : AbstractValidator<GetAllCompaniesQuery>
{
    public GetAllCompaniesQueryValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThan(0).WithMessage("Limit must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Limit cannot exceed 100.");
    }
}
