using Arda9UserApi.Application.DTOs;

namespace Arda9UserApi.Application.Companies.GetAllCompanies;

public class GetAllCompaniesQueryResponse
{
    public List<CompanyDto> Companies { get; set; } = new();
    public int Count { get; set; }
}
