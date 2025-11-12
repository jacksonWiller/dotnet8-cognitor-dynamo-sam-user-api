using Ardalis.Result;
using MediatR;

namespace Arda9UserApi.Application.Companies.GetAllCompanies;

public class GetAllCompaniesQuery : IRequest<Result<GetAllCompaniesQueryResponse>>
{
    public int Limit { get; set; } = 10;
}
