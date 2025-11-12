using Catalog.Domain.Entities.CompanyAggregate;

namespace Arda9UserApi.Infrastructure.Repositories;

/// <summary>
/// Repositório de Company para operações CRUD no DynamoDB
/// </summary>
public interface ICompanyRepository
{
    /// <summary>
    /// Adiciona uma nova company ao DynamoDB
    /// </summary>
    /// <param name="company">Entidade de domínio Company</param>
    /// <returns>success/failure</returns>
    Task<bool> CreateAsync(Company company);

    /// <summary>
    /// Remove uma company existente do DynamoDB
    /// </summary>
    /// <param name="company">Entidade de domínio Company</param>
    /// <returns>success/failure</returns>
    Task<bool> DeleteAsync(Company company);

    /// <summary>
    /// Lista companies do DynamoDB com limite de items (padrão=10)
    /// </summary>
    /// <param name="limit">limite (padrão=10)</param>
    /// <returns>Coleção de companies do domínio</returns>
    Task<IList<Company>> GetCompaniesAsync(int limit = 10);

    /// <summary>
    /// Obtém uma company pela chave primária
    /// </summary>
    /// <param name="id">ID da company</param>
    /// <returns>Entidade de domínio Company</returns>
    Task<Company?> GetByIdAsync(Guid id);

    /// <summary>
    /// Obtém uma company pelo slug
    /// </summary>
    /// <param name="slug">Slug da company</param>
    /// <returns>Entidade de domínio Company</returns>
    Task<Company?> GetBySlugAsync(string slug);

    /// <summary>
    /// Atualiza o conteúdo de uma company
    /// </summary>
    /// <param name="company">Entidade de domínio Company</param>
    /// <returns>success/failure</returns>
    Task<bool> UpdateAsync(Company company);
}
