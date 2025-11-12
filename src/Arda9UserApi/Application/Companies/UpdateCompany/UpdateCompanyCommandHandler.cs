using Arda9UserApi.Application.DTOs;
using Arda9UserApi.Infrastructure.Repositories;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using Catalog.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Arda9UserApi.Application.Companies.UpdateCompany;

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Result<UpdateCompanyResponse>>
{
    private readonly IValidator<UpdateCompanyCommand> _validator;
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateCompanyCommandHandler(
        IValidator<UpdateCompanyCommand> validator,
        ICompanyRepository companyRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _validator = validator;
        _companyRepository = companyRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<UpdateCompanyResponse>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<UpdateCompanyResponse>.Invalid(validationResult.AsErrors());
        }

        var existingCompany = await _companyRepository.GetByIdAsync(request.Id);
        if (existingCompany == null)
        {
            return Result<UpdateCompanyResponse>.NotFound($"Company with Id {request.Id} not found");
        }

        // Obter o userId do usuário autenticado (se disponível)
        Guid? updatedBy = null;
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
        {
            updatedBy = userId;
        }

        // Atualizar informações básicas da company se fornecidas
        if (request.Name != null || request.Email != null || request.Phone != null || request.Address != null || request.Tags != null)
        {
            var companyName = request.Name != null ? new CompanyName(request.Name) : existingCompany.Name;

            Email? email = existingCompany.Email;
            if (request.Email != null)
            {
                email = !string.IsNullOrEmpty(request.Email) ? new Email(request.Email) : null;
            }

            Phone? phone = existingCompany.Phone;
            if (request.Phone != null)
            {
                phone = !string.IsNullOrEmpty(request.Phone.CountryCode) && !string.IsNullOrEmpty(request.Phone.Number)
                    ? new Phone(request.Phone.CountryCode, request.Phone.Number)
                    : null;
            }

            Address? address = existingCompany.Address;
            if (request.Address != null)
            {
                address = new Address(
                    request.Address.Street,
                    request.Address.Number,
                    request.Address.City,
                    request.Address.State,
                    request.Address.PostalCode,
                    request.Address.Country,
                    request.Address.Complement,
                    request.Address.District
                );
            }

            var tags = request.Tags ?? existingCompany.Tags;

            existingCompany.Update(
                name: companyName,
                email: email,
                phone: phone,
                address: address,
                tags: tags,
                updatedBy: updatedBy
            );
        }

        // Atualizar settings se fornecidas
        if (request.Settings != null)
        {
            var settings = new CompanySettings(
                request.Settings.SelfRegister,
                request.Settings.MfaRequired,
                request.Settings.DomainsAllowed
            );

            existingCompany.UpdateSettings(settings, updatedBy);
        }

        var updateSuccess = await _companyRepository.UpdateAsync(existingCompany);
        if (!updateSuccess)
        {
            return Result<UpdateCompanyResponse>.Error(/*"Failed to update company."*/);
        }

        var companyDto = _mapper.Map<CompanyDto>(existingCompany);
        var response = new UpdateCompanyResponse
        {
            Company = companyDto
        };

        return Result<UpdateCompanyResponse>.Success(response, "Company updated successfully.");
    }
}
