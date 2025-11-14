using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Arda9UserApi.Application.DTOs;
using AutoMapper;
using Catalog.Domain.Entities.CompanyAggregate;

namespace Arda9UserApi.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly IDynamoDBContext context;
    private readonly ILogger<CompanyRepository> logger;
    private readonly IMapper mapper;

    public CompanyRepository(IDynamoDBContext context, ILogger<CompanyRepository> logger, IMapper mapper)
    {
        this.context = context;
        this.logger = logger;
        this.mapper = mapper;
    }

    public async Task<bool> CreateAsync(Company company)
    {
        try
        {
            // Mapeia a entidade de domínio para o DTO
            var companyDto = mapper.Map<CompanyDto>(company);
            companyDto.CreatedAt = DateTime.UtcNow;
            companyDto.UpdatedAt = DateTime.UtcNow;

            await context.SaveAsync(companyDto);
            logger.LogInformation("Company {Id} with slug {Slug} is added", company.Id, company.Slug.Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "fail to persist company to DynamoDb Table");
            return false;
        }

        return true;
    }

    public async Task<bool> DeleteAsync(Company company)
    {
        bool result;
        try
        {
            // Deleta a company usando PK e SK
            var pk = $"COMPANY#{company.Id}";
            var sk = "METADATA";

            await context.DeleteAsync<CompanyDto>(pk, sk);

            // Tenta recuperar a company deletada. Deve retornar null.
            //CompanyDto? deletedCompany = await context.LoadAsync<CompanyDto>(pk, sk, new DynamoDBContextConfig
            //{
            //    ConsistentRead = true
            //});

            result = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "fail to delete company from DynamoDb Table");
            result = false;
        }

        if (result) logger.LogInformation("Company {Id} is deleted", company.Id);

        return result;
    }

    public async Task<bool> UpdateAsync(Company company)
    {
        if (company == null) return false;

        try
        {
            // Mapeia a entidade de domínio para o DTO
            var companyDto = mapper.Map<CompanyDto>(company);
            companyDto.UpdatedAt = DateTime.UtcNow;

            await context.SaveAsync(companyDto);
            logger.LogInformation("Company {Id} with slug {Slug} is updated", company.Id, company.Slug.Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "fail to update company from DynamoDb Table");
            return false;
        }

        return true;
    }

    public async Task<Company?> GetByIdAsync(Guid id)
    {
        try
        {
            var pk = $"COMPANY#{id}";
            var sk = "METADATA";

            var companyDto = await context.LoadAsync<CompanyDto>(pk, sk);

            if (companyDto == null) return null;

            // Mapeia o DTO para a entidade de domínio
            return mapper.Map<Company>(companyDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "fail to get company from DynamoDb Table");
            return null;
        }
    }

    public async Task<Company?> GetBySlugAsync(string slug)
    {
        try
        {
            var filter = new ScanFilter();
            filter.AddCondition("Slug", ScanOperator.Equal, slug);

            var scanConfig = new ScanOperationConfig()
            {
                Limit = 1,
                Filter = filter
            };

            var queryResult = context.FromScanAsync<CompanyDto>(scanConfig);
            var companies = await queryResult.GetNextSetAsync();

            var companyDto = companies.FirstOrDefault();

            if (companyDto == null) return null;

            // Mapeia o DTO para a entidade de domínio
            return mapper.Map<Company>(companyDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "fail to get company by slug from DynamoDb Table");
            return null;
        }
    }

    public async Task<IList<Company>> GetCompaniesAsync(int limit = 10)
    {
        var result = new List<Company>();

        try
        {
            if (limit <= 0)
            {
                return result;
            }

            var scanConfig = new ScanOperationConfig()
            {
                Limit = limit
            };

            var queryResult = context.FromScanAsync<CompanyDto>(scanConfig);

            do
            {
                var dtos = await queryResult.GetNextSetAsync();

                // Mapeia os DTOs para entidades de domínio
                var companies = mapper.Map<List<Company>>(dtos);
                result.AddRange(companies);
            }
            while (!queryResult.IsDone && result.Count < limit);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "fail to list companies from DynamoDb Table");
            return new List<Company>();
        }

        return result;
    }
}
