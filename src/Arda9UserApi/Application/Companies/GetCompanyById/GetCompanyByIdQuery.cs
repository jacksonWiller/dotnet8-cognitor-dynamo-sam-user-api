using Ardalis.Result;
using MediatR;

namespace Arda9UserApi.Application.Companies.GetCompanyById;

public class GetCompanyByIdQuery : IRequest<Result<GetCompanyByIdQueryResponse>>
{
    public Guid Id { get; set; }

    public GetCompanyByIdQuery(Guid id)
    {
        Id = id;
    }
}
