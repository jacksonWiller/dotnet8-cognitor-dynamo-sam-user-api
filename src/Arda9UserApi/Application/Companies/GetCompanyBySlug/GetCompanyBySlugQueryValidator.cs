using FluentValidation;

namespace Arda9UserApi.Application.Companies.GetCompanyBySlug;

public class GetCompanyBySlugQueryValidator : AbstractValidator<GetCompanyBySlugQuery>
{
    public GetCompanyBySlugQueryValidator()
    {
        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MinimumLength(3).WithMessage("Slug must be at least 3 characters.")
            .MaximumLength(60).WithMessage("Slug must be up to 60 characters.")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("Slug must contain only lowercase letters, numbers, and hyphens.");
    }
}
