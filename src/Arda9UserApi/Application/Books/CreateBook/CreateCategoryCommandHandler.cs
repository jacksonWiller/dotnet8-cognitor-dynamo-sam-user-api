using Arda9UserApi.Application.DTOs;
using Arda9UserApi.Infrastructure.Repositories;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using Catalog.Domain.Entities.BookAggregate;
using FluentValidation;
using MediatR;

namespace Arda9UserApi.Application.Books.CreateBook;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, Result<CreateBookResponse>>
{
    private readonly IValidator<CreateBookCommand> _validator;
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;

    public CreateBookCommandHandler(
        IValidator<CreateBookCommand> validator,
        IBookRepository bookRepository,
        IMapper mapper
    )
    {
        _validator = validator;
        _bookRepository = bookRepository;
        _mapper = mapper;
    }

    public async Task<Result<CreateBookResponse>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<CreateBookResponse>.Invalid(validationResult.AsErrors());
        }

        // Cria a entidade de domínio Book
        var book = new Book(
            name: request.Name,
            description: request.Description,
            price: request.Price,
            stockQuantity: request.StockQuantity,
            sku: request.SKU,
            brand: request.Brand
        );

        // TODO: Se CategoryIds foram fornecidos, buscar as categorias e adicionar ao book
        // Por enquanto, o book será criado sem categorias

        await _bookRepository.CreateAsync(book);

        var bookDto = _mapper.Map<BookDto>(book);
        var response = new CreateBookResponse
        {
            Book = bookDto
        };

        return Result<CreateBookResponse>.Success(response, "Book created successfully.");
    }
}