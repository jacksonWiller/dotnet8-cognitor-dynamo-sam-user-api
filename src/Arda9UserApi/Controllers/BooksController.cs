using Arda9UserApi.Core.CQRS;
using Arda9UserApi.Entities;
using Arda9UserApi.Features.Books.Commands;
using Arda9UserApi.Features.Books.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Arda9UserApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class BooksController : ControllerBase
{
    private readonly ILogger<BooksController> _logger;
    private readonly IMediator _mediator;

    public BooksController(ILogger<BooksController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém uma lista de livros com limite especificado
    /// </summary>
    /// <param name="limit">Número máximo de livros a retornar (1-100)</param>
    /// <returns>Lista de livros</returns>
    /// <response code="200">Retorna a lista de livros</response>
    /// <response code="400">Se o limite estiver fora do intervalo válido</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Book>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<Book>>> Get([FromQuery][Range(1, 100)] int limit = 10)
    {
        var query = new GetBooksQuery(limit);
        var response = await _mediator.SendAsync(query);

        if (!response.Success)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(response.Books);
    }

    /// <summary>
    /// Obtém um livro específico pelo ID
    /// </summary>
    /// <param name="id">ID único do livro</param>
    /// <returns>Dados do livro</returns>
    /// <response code="200">Retorna o livro encontrado</response>
    /// <response code="404">Se o livro não for encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Book>> Get(Guid id)
    {
        var query = new GetBookByIdQuery(id);
        var response = await _mediator.SendAsync(query);

        if (!response.Success)
        {
            if (response.NotFound)
            {
                return NotFound();
            }
            return BadRequest(response.ErrorMessage);
        }

        return Ok(response.Book);
    }

    /// <summary>
    /// Cria um novo livro
    /// </summary>
    /// <param name="book">Dados do livro a ser criado</param>
    /// <returns>Livro criado</returns>
    /// <response code="201">Livro criado com sucesso</response>
    /// <response code="400">Se os dados do livro forem inválidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(Book), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Book>> Post([FromBody] Book book)
    {
        var command = new CreateBookCommand(book);
        var response = await _mediator.SendAsync(command);

        if (!response.Success)
        {
            return BadRequest(response.ErrorMessage);
        }

        return CreatedAtAction(
            nameof(Get),
            new { id = response.Book!.Id },
            response.Book);
    }

    /// <summary>
    /// Atualiza um livro existente
    /// </summary>
    /// <param name="id">ID do livro a ser atualizado</param>
    /// <param name="book">Novos dados do livro</param>
    /// <returns>Resultado da operação</returns>
    /// <response code="200">Livro atualizado com sucesso</response>
    /// <response code="400">Se os dados forem inválidos</response>
    /// <response code="404">Se o livro não for encontrado</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(Guid id, [FromBody] Book book)
    {
        var command = new UpdateBookCommand(id, book);
        var response = await _mediator.SendAsync(command);

        if (!response.Success)
        {
            if (response.ErrorMessage?.Contains("No book found") == true)
            {
                return NotFound(response.ErrorMessage);
            }
            return BadRequest(response.ErrorMessage);
        }

        return Ok();
    }

    /// <summary>
    /// Remove um livro
    /// </summary>
    /// <param name="id">ID do livro a ser removido</param>
    /// <returns>Resultado da operação</returns>
    /// <response code="200">Livro removido com sucesso</response>
    /// <response code="400">Se o ID for inválido</response>
    /// <response code="404">Se o livro não for encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteBookCommand(id);
        var response = await _mediator.SendAsync(command);

        if (!response.Success)
        {
            if (response.ErrorMessage?.Contains("No book found") == true)
            {
                return NotFound(response.ErrorMessage);
            }
            return BadRequest(response.ErrorMessage);
        }

        return Ok();
    }
}
