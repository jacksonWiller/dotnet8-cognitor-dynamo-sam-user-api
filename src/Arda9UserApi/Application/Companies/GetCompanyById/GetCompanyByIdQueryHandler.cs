using Arda9UserApi.Application.DTOs;
using Arda9UserApi.Infrastructure.Repositories;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Arda9UserApi.Application.Companies.GetCompanyById;

public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, Result<GetCompanyByIdQueryResponse>>
{
    private readonly IValidator<GetCompanyByIdQuery> _validator;
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;

    public GetCompanyByIdQueryHandler(
        IValidator<GetCompanyByIdQuery> validator,
        ICompanyRepository companyRepository,
        IMapper mapper
    )
    {
        _validator = validator;
        _companyRepository = companyRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetCompanyByIdQueryResponse>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<GetCompanyByIdQueryResponse>.Invalid(validationResult.AsErrors());
        }

        var company = await _companyRepository.GetByIdAsync(request.Id);

        if (company == null)
        {
            return Result<GetCompanyByIdQueryResponse>.NotFound($"Company with ID {request.Id} not found.");
        }

        var companyDto = _mapper.Map<CompanyDto>(company);
        var response = new GetCompanyByIdQueryResponse { Company = companyDto };

        return Result<GetCompanyByIdQueryResponse>.Success(response, "Company retrieved successfully.");
    }
}
