using Arda9UserApi.Application.DTOs;
using Arda9UserApi.Infrastructure.Repositories;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Arda9UserApi.Application.Books.GetAllBooks;

public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, Result<GetAllBooksQueryResponse>>
{
    private readonly IValidator<GetAllBooksQuery> _validator;
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;

    public GetAllBooksQueryHandler(
        IValidator<GetAllBooksQuery> validator,
        IBookRepository bookRepository,
        IMapper mapper
    )
    {
        _validator = validator;
        _bookRepository = bookRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetAllBooksQueryResponse>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<GetAllBooksQueryResponse>.Invalid(validationResult.AsErrors());
        }

        var books = await _bookRepository.GetBooksAsync(request.Limit);
        var bookDtos = _mapper.Map<List<BookDto>>(books);
        var response = new GetAllBooksQueryResponse() { Books = bookDtos, Count = bookDtos.Count };

        return Result<GetAllBooksQueryResponse>.Success(response, "Books retrieved successfully.");
    }
}