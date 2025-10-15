using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using Arda9UserApi.Entities;
using Arda9UserApi.Repositories;
using System.ComponentModel.DataAnnotations;

namespace Arda9UserApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class BooksController : ControllerBase
{
    private readonly ILogger<BooksController> logger;
    private readonly IBookRepository bookRepository;

    public BooksController(ILogger<BooksController> logger, IBookRepository bookRepository)
    {
        this.logger = logger;
        this.bookRepository = bookRepository;
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
        if (limit <= 0 || limit > 100) return BadRequest("The limit should been between [1-100]");

        return Ok(await bookRepository.GetBooksAsync(limit));
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
        var result = await bookRepository.GetByIdAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
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
        if (book == null) return ValidationProblem("Invalid input! Book not informed");

        var result = await bookRepository.CreateAsync(book);

        if (result)
        {
            return CreatedAtAction(
                nameof(Get),
                new { id = book.Id },
                book);
        }
        else
        {
            return BadRequest("Fail to persist");
        }
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
        if (id == Guid.Empty || book == null) return ValidationProblem("Invalid request payload");

        // Retrieve the book.
        var bookRetrieved = await bookRepository.GetByIdAsync(id);

        if (bookRetrieved == null)
        {
            var errorMsg = $"Invalid input! No book found with id:{id}";
            return NotFound(errorMsg);
        }

        book.Id = bookRetrieved.Id;

        await bookRepository.UpdateAsync(book);
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
        if (id == Guid.Empty) return ValidationProblem("Invalid request payload");

        var bookRetrieved = await bookRepository.GetByIdAsync(id);

        if (bookRetrieved == null)
        {
            var errorMsg = $"Invalid input! No book found with id:{id}";
            return NotFound(errorMsg);
        }

        await bookRepository.DeleteAsync(bookRetrieved);
        return Ok();
    }
}
