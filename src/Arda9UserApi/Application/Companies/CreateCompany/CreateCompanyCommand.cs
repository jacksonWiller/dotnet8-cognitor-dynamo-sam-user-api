using Ardalis.Result;
using Arda9UserApi.Application.DTOs;
using MediatR;

namespace Arda9UserApi.Application.Companies.CreateCompany;

public class CreateCompanyCommand : IRequest<Result<CreateCompanyResponse>>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Document { get; set; }
    public string? DocumentCountry { get; set; } = "BR";
    public string? Email { get; set; }
    public PhoneDto? Phone { get; set; }
    public AddressDto? Address { get; set; }
    public List<string>? Tags { get; set; }
    public CompanySettingsDto? Settings { get; set; }
}
