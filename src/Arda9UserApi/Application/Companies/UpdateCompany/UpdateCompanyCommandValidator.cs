using FluentValidation;

namespace Arda9UserApi.Application.Companies.UpdateCompany;

public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Company ID is required.");

        RuleFor(x => x.Name)
            .MinimumLength(2).WithMessage("Company name must be at least 2 characters.")
            .MaximumLength(120).WithMessage("Company name must be up to 120 characters.")
            .When(x => !string.IsNullOrEmpty(x.Name));

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
