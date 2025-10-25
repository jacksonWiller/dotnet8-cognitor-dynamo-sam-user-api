using Arda9UserApi.Application.DTOs;
using Arda9UserApi.Infrastructure.Repositories;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Arda9UserApi.Application.Books.UpdateBook;

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, Result<UpdateBookResponse>>
{
    private readonly IValidator<UpdateBookCommand> _validator;
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;

    public UpdateBookCommandHandler(
        IValidator<UpdateBookCommand> validator,
        IBookRepository bookRepository,
        IMapper mapper
    )
    {
        _validator = validator;
        _bookRepository = bookRepository;
        _mapper = mapper;
    }

    public async Task<Result<UpdateBookResponse>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<UpdateBookResponse>.Invalid(validationResult.AsErrors());
        }

        var existingBook = await _bookRepository.GetByIdAsync(request.Id);
        if (existingBook == null)
        {
            return Result<UpdateBookResponse>.NotFound($"Book with Id {request.Id} not found");
        }

        // Atualiza o book usando o método Update da entidade de domínio
        existingBook.Update(
            name: request.Name ?? existingBook.Name,
            description: request.Description ?? existingBook.Description,
            price: request.Price ?? existingBook.Price,
            stockQuantity: request.StockQuantity ?? existingBook.StockQuantity,
            sku: request.SKU ?? existingBook.SKU,
            brand: request.Brand ?? existingBook.Brand
        );

        // TODO: Se CategoryIds foram fornecidos, atualizar as categorias

        var updateSuccess = await _bookRepository.UpdateAsync(existingBook);
        if (!updateSuccess)
        {
            return Result<UpdateBookResponse>.Error();
        }

        var bookDto = _mapper.Map<BookDto>(existingBook);
        var response = new UpdateBookResponse
        {
            Book = bookDto
        };

        return Result<UpdateBookResponse>.Success(response, "Book updated successfully.");
    }
}
