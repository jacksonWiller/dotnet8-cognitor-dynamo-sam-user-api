using FluentValidation;

namespace Arda9UserApi.Application.Companies.CreateCompany;

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Company name is required.")
            .MinimumLength(2).WithMessage("Company name must be at least 2 characters.")
            .MaximumLength(120).WithMessage("Company name must be up to 120 characters.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MinimumLength(3).WithMessage("Slug must be at least 3 characters.")
            .MaximumLength(60).WithMessage("Slug must be up to 60 characters.")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("Slug must contain only lowercase letters, numbers, and hyphens.");

        RuleFor(x => x.Document)
            .MaximumLength(20).WithMessage("Document must be up to 20 characters.")
            .When(x => !string.IsNullOrEmpty(x.Document));

        RuleFor(x => x.DocumentCountry)
            .Length(2).WithMessage("Document country must be a 2-letter country code (e.g., BR, US).")
            .When(x => !string.IsNullOrEmpty(x.DocumentCountry));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(254).WithMessage("Email cannot exceed 254 characters.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Phone)
            .Must(phone => !string.IsNullOrEmpty(phone!.CountryCode) && !string.IsNullOrEmpty(phone.Number))
            .WithMessage("Phone must have both country code and number.")
            .When(x => x.Phone != null);

        RuleFor(x => x.Address)
            .Must(address => !string.IsNullOrEmpty(address!.Street) &&
                           !string.IsNullOrEmpty(address.Number) &&
                           !string.IsNullOrEmpty(address.City) &&
                           !string.IsNullOrEmpty(address.State) &&
                           !string.IsNullOrEmpty(address.PostalCode) &&
                           !string.IsNullOrEmpty(address.Country))
            .WithMessage("Address must have Street, Number, City, State, PostalCode and Country.")
            .When(x => x.Address != null);

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count <= 20)
            .WithMessage("Cannot have more than 20 tags.");
    }
}
