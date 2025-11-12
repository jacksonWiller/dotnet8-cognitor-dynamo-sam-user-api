using Ardalis.Result;
using MediatR;

namespace Arda9UserApi.Application.Companies.GetCompanyBySlug;

public class GetCompanyBySlugQuery : IRequest<Result<GetCompanyBySlugQueryResponse>>
{
    public string Slug { get; set; }

    public GetCompanyBySlugQuery(string slug)
    {
        Slug = slug;
    }
}
