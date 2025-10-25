using Catalog.Domain.Entities.BookAggregate;

namespace Arda9UserApi.Infrastructure.Repositories;

/// <summary>
/// Repositório de Book para operações CRUD no DynamoDB
/// </summary>
public interface IBookRepository
{
    /// <summary>
    /// Adiciona um novo book ao DynamoDB
    /// </summary>
    /// <param name="book">Entidade de domínio Book</param>
    /// <returns>success/failure</returns>
    Task<bool> CreateAsync(Book book);

    /// <summary>
    /// Remove um book existente do DynamoDB
    /// </summary>
    /// <param name="book">Entidade de domínio Book</param>
    /// <returns>success/failure</returns>
    Task<bool> DeleteAsync(Book book);

    /// <summary>
    /// Lista books do DynamoDB com limite de items (padrão=10)
    /// </summary>
    /// <param name="limit">limite (padrão=10)</param>
    /// <returns>Coleção de books do domínio</returns>
    Task<IList<Book>> GetBooksAsync(int limit = 10);

    /// <summary>
    /// Obtém um book pela chave primária
    /// </summary>
    /// <param name="id">ID do book</param>
    /// <returns>Entidade de domínio Book</returns>
    Task<Book?> GetByIdAsync(Guid id);

    /// <summary>
    /// Atualiza o conteúdo de um book
    /// </summary>
    /// <param name="book">Entidade de domínio Book</param>
    /// <returns>success/failure</returns>
    Task<bool> UpdateAsync(Book book);
}