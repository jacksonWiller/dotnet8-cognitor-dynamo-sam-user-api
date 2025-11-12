using Arda9UserApi.Application.DTOs;
using Arda9UserApi.Infrastructure.Repositories;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using Catalog.Domain.Entities.CompanyAggregate;
using Catalog.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Arda9UserApi.Application.Companies.CreateCompany;

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Result<CreateCompanyResponse>>
{
    private readonly IValidator<CreateCompanyCommand> _validator;
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateCompanyCommandHandler(
        IValidator<CreateCompanyCommand> validator,
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

    public async Task<Result<CreateCompanyResponse>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<CreateCompanyResponse>.Invalid(validationResult.AsErrors());
        }

        // Verificar se já existe uma company com o mesmo slug
        var existingCompany = await _companyRepository.GetBySlugAsync(request.Slug);
        if (existingCompany != null)
        {
            //return Result<CreateCompanyResponse>.Error($"A company with slug '{request.Slug}' already exists.");
            return Result<CreateCompanyResponse>.Error();

        }

        // Obter o userId do usuário autenticado (se disponível)
        Guid? createdBy = null;
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
        {
            createdBy = userId;
        }

        // Criar os Value Objects
        var companyName = new CompanyName(request.Name);
        var slug = new Slug(request.Slug);

        CompanyDocument? document = null;
        if (!string.IsNullOrEmpty(request.Document) && !string.IsNullOrEmpty(request.DocumentCountry))
        {
            document = new CompanyDocument(request.Document, request.DocumentCountry);
        }

        Email? email = null;
        if (!string.IsNullOrEmpty(request.Email))
        {
            email = new Email(request.Email);
        }

        Phone? phone = null;
        if (request.Phone != null && !string.IsNullOrEmpty(request.Phone.CountryCode) && !string.IsNullOrEmpty(request.Phone.Number))
        {
            phone = new Phone(request.Phone.CountryCode, request.Phone.Number);
        }

        Address? address = null;
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

        CompanySettings? settings = null;
        if (request.Settings != null)
        {
            settings = new CompanySettings(
                request.Settings.SelfRegister,
                request.Settings.MfaRequired,
                request.Settings.DomainsAllowed
            );
        }

        // Cria a entidade de domínio Company
        var company = new Company(
            name: companyName,
            slug: slug,
            document: document,
            email: email,
            phone: phone,
            address: address,
            tags: request.Tags,
            settings: settings,
            createdBy: createdBy
        );

        var createSuccess = await _companyRepository.CreateAsync(company);
        if (!createSuccess)
        {
            //return Result<CreateCompanyResponse>.Error("Failed to create company.");
            return Result<CreateCompanyResponse>.Error();
        }

        var companyDto = _mapper.Map<CompanyDto>(company);
        var response = new CreateCompanyResponse
        {
            Company = companyDto
        };

        return Result<CreateCompanyResponse>.Success(response, "Company created successfully.");
    }
}
