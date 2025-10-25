using Arda9UserApi.Application.DTOs;
using Arda9UserApi.Infrastructure.Repositories;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Arda9UserApi.Application.Books.GetBookById;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, Result<GetBookByIdQueryResponse>>
{
    private readonly IValidator<GetBookByIdQuery> _validator;
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;

    public GetBookByIdQueryHandler(
        IValidator<GetBookByIdQuery> validator,
        IBookRepository bookRepository,
        IMapper mapper
    )
    {
        _validator = validator;
        _bookRepository = bookRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetBookByIdQueryResponse>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<GetBookByIdQueryResponse>.Invalid(validationResult.AsErrors());
        }

        var book = await _bookRepository.GetByIdAsync(request.Id);

        if (book == null)
        {
            return Result<GetBookByIdQueryResponse>.NotFound($"Book with ID {request.Id} not found.");
        }

        var bookDto = _mapper.Map<BookDto>(book);
        var response = new GetBookByIdQueryResponse { Book = bookDto };

        return Result<GetBookByIdQueryResponse>.Success(response, "Book retrieved successfully.");
    }
}
