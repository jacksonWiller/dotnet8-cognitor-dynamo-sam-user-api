using Arda9UserApi.Application.DTOs;
using Arda9UserApi.Infrastructure.Repositories;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Arda9UserApi.Application.Companies.GetCompanyBySlug;

public class GetCompanyBySlugQueryHandler : IRequestHandler<GetCompanyBySlugQuery, Result<GetCompanyBySlugQueryResponse>>
{
    private readonly IValidator<GetCompanyBySlugQuery> _validator;
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;

    public GetCompanyBySlugQueryHandler(
        IValidator<GetCompanyBySlugQuery> validator,
        ICompanyRepository companyRepository,
        IMapper mapper
    )
    {
        _validator = validator;
        _companyRepository = companyRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetCompanyBySlugQueryResponse>> Handle(GetCompanyBySlugQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<GetCompanyBySlugQueryResponse>.Invalid(validationResult.AsErrors());
        }

        var company = await _companyRepository.GetBySlugAsync(request.Slug);

        if (company == null)
        {
            return Result<GetCompanyBySlugQueryResponse>.NotFound($"Company with slug '{request.Slug}' not found.");
        }

        var companyDto = _mapper.Map<CompanyDto>(company);
        var response = new GetCompanyBySlugQueryResponse { Company = companyDto };

        return Result<GetCompanyBySlugQueryResponse>.Success(response, "Company retrieved successfully.");
    }
}
