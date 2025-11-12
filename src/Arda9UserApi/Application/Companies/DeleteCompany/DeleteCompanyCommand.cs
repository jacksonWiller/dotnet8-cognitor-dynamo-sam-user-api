using Ardalis.Result;
using MediatR;

namespace Arda9UserApi.Application.Companies.DeleteCompany;

public class DeleteCompanyCommand : IRequest<Result<DeleteCompanyResponse>>
{
    public Guid Id { get; set; }

    public DeleteCompanyCommand(Guid id)
    {
        Id = id;
    }
}
