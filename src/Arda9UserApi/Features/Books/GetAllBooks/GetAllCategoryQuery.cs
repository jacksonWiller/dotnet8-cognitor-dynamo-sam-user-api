using Ardalis.Result;
using MediatR;
namespace Arda9UserApi.Features.Books.GetAllBooks;
public class GetAllBooksQuery : IRequest<Result<GetAllBooksQueryResponse>>
{
    public int Limit { get; set; }

}
