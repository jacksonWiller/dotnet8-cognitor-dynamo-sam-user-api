using Arda9UserApi.Infrastructure.Repositories;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;

namespace Arda9UserApi.Application.Companies.DeleteCompany;

public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, Result<DeleteCompanyResponse>>
{
    private readonly IValidator<DeleteCompanyCommand> _validator;
    private readonly ICompanyRepository _companyRepository;

    public DeleteCompanyCommandHandler(
        IValidator<DeleteCompanyCommand> validator,
        ICompanyRepository companyRepository
    )
    {
        _validator = validator;
        _companyRepository = companyRepository;
    }

    public async Task<Result<DeleteCompanyResponse>> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<DeleteCompanyResponse>.Invalid(validationResult.AsErrors());
        }

        var existingCompany = await _companyRepository.GetByIdAsync(request.Id);
        if (existingCompany == null)
        {
            return Result<DeleteCompanyResponse>.NotFound($"Company with Id {request.Id} not found");
        }

        var deleteSuccess = await _companyRepository.DeleteAsync(existingCompany);
        if (!deleteSuccess)
        {
            return Result<DeleteCompanyResponse>.Error(
                //"Failed to delete company."
                );
        }

        var response = new DeleteCompanyResponse
        {
            Id = request.Id,
            Message = "Company deleted successfully"
        };

        return Result<DeleteCompanyResponse>.Success(response, "Company deleted successfully.");
    }
}
