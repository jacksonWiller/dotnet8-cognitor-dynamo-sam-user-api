using Arda9UserApi.Application.DTOs;
using Arda9UserApi.Infrastructure.Repositories;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Arda9UserApi.Application.Companies.GetAllCompanies;

public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, Result<GetAllCompaniesQueryResponse>>
{
    private readonly IValidator<GetAllCompaniesQuery> _validator;
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;

    public GetAllCompaniesQueryHandler(
        IValidator<GetAllCompaniesQuery> validator,
        ICompanyRepository companyRepository,
        IMapper mapper
    )
    {
        _validator = validator;
        _companyRepository = companyRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetAllCompaniesQueryResponse>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<GetAllCompaniesQueryResponse>.Invalid(validationResult.AsErrors());
        }

        var companies = await _companyRepository.GetCompaniesAsync(request.Limit);
        var companyDtos = _mapper.Map<List<CompanyDto>>(companies);
        var response = new GetAllCompaniesQueryResponse() { Companies = companyDtos, Count = companyDtos.Count };

        return Result<GetAllCompaniesQueryResponse>.Success(response, "Companies retrieved successfully.");
    }
}
