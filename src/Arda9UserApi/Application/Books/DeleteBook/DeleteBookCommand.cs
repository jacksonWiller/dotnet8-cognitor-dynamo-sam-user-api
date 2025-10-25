using Ardalis.Result;
using MediatR;

namespace Arda9UserApi.Application.Books.DeleteBook;

public class DeleteBookCommand : IRequest<Result<DeleteBookResponse>>
{
    public Guid Id { get; set; }

    public DeleteBookCommand(Guid id)
    {
        Id = id;
    }
}
