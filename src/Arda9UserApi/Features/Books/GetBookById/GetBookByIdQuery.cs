using Ardalis.Result;
using MediatR;

namespace Arda9UserApi.Features.Books.GetBookById;

public class GetBookByIdQuery : IRequest<Result<GetBookByIdQueryResponse>>
{
    public Guid Id { get; set; }

    public GetBookByIdQuery(Guid id)
    {
        Id = id;
    }
}
