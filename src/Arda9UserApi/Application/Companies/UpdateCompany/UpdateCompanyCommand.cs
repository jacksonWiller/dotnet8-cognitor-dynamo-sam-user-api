using Ardalis.Result;
using Arda9UserApi.Application.DTOs;
using MediatR;

namespace Arda9UserApi.Application.Companies.UpdateCompany;

public class UpdateCompanyCommand : IRequest<Result<UpdateCompanyResponse>>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public PhoneDto? Phone { get; set; }
    public AddressDto? Address { get; set; }
    public List<string>? Tags { get; set; }
    public CompanySettingsDto? Settings { get; set; }
}
