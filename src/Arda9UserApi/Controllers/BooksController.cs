using Arda9UserApi.Api.Extensions;
using Arda9UserApi.Features.Books.CreateBook;
using Arda9UserApi.Features.Books.GetAllBooks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arda9UserApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém uma lista de livros com limite especificado
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(GetAllBooksQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get([FromQuery] GetAllBooksQuery query)
    {
        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }

    /// <summary>
    /// Cria um novo livro
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateBookResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateBookCommand command)
    {
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Obtém um livro específico pelo ID
    /// </summary>
    //[HttpGet("{id}")]
    //[ProducesResponseType(typeof(GetBookByIdResponse), StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Get(Guid id)
    //{
    //    var result = await _mediator.Send(new GetBookByIdQuery(id));
    //    return result.ToActionResult();
    //}

    

    /// <summary>
    /// Atualiza um livro existente
    /// </summary>
    //[HttpPut("{id}")]
    //[ProducesResponseType(typeof(UpdateBookResponse), StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Put(Guid id, [FromBody] UpdateBookCommand command)
    //{
    //    command.Id = id;
    //    var result = await _mediator.Send(command);
    //    return result.ToActionResult();
    //}

    /// <summary>
    /// Remove um livro
    /// </summary>
    //[HttpDelete("{id}")]
    //[ProducesResponseType(typeof(DeleteBookResponse), StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Delete(Guid id)
    //{
    //    var result = await _mediator.Send(new DeleteBookCommand(id));
    //    return result.ToActionResult();
    //}
}