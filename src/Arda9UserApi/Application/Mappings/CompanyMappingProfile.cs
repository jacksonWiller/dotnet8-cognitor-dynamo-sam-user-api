using AutoMapper;
using Catalog.Domain.Entities.CompanyAggregate;
using Catalog.Domain.ValueObjects;
using Catalog.Domain.Enums;
using Arda9UserApi.Application.DTOs;

namespace Arda9UserApi.Application.Mappings;

/// <summary>
/// Perfil de mapeamento do AutoMapper entre entidades de domínio de Company e DTOs
/// </summary>
public class CompanyMappingProfile : Profile
{
    public CompanyMappingProfile()
    {
        // Mapeamento bidirecional de Company (Domain) para CompanyDto (usado para API e persistência)
        CreateMap<Company, CompanyDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Value))
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug.Value))
            .ForMember(dest => dest.Document, opt => opt.MapFrom(src => src.Document != null ? src.Document.Value : null))
            .ForMember(dest => dest.DocumentCountry, opt => opt.MapFrom(src => src.Document != null ? src.Document.Country : null))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email != null ? src.Email.Value : null))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => src.Settings))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
            .ReverseMap()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => new CompanyName(src.Name)))
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => new Slug(src.Slug)))
            .ForMember(dest => dest.Document, opt => opt.MapFrom(src =>
                !string.IsNullOrEmpty(src.Document) && !string.IsNullOrEmpty(src.DocumentCountry)
                    ? new CompanyDocument(src.Document, src.DocumentCountry)
                    : null))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src =>
                !string.IsNullOrEmpty(src.Email) ? new Email(src.Email) : null))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<CompanyStatus>(src.Status)))
            .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => src.Settings))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy));

        // Mapeamento bidirecional de Phone (ValueObject) para PhoneDto
        CreateMap<Phone, PhoneDto>()
            .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.CountryCode))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ReverseMap()
            .ConstructUsing(src => new Phone(src.CountryCode, src.Number));

        // Mapeamento bidirecional de Address (ValueObject) para AddressDto
        CreateMap<Address, AddressDto>()
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.Complement, opt => opt.MapFrom(src => src.Complement))
            .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.District))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
            .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
            .ReverseMap()
            .ConstructUsing(src => new Address(
                src.Street,
                src.Number,
                src.City,
                src.State,
                src.PostalCode,
                src.Country,
                src.Complement,
                src.District));

        // Mapeamento bidirecional de CompanySettings (ValueObject) para CompanySettingsDto
        CreateMap<CompanySettings, CompanySettingsDto>()
            .ForMember(dest => dest.SelfRegister, opt => opt.MapFrom(src => src.SelfRegister))
            .ForMember(dest => dest.MfaRequired, opt => opt.MapFrom(src => src.MfaRequired))
            .ForMember(dest => dest.DomainsAllowed, opt => opt.MapFrom(src => src.DomainsAllowed))
            .ReverseMap()
            .ConstructUsing(src => new CompanySettings(
                src.SelfRegister,
                src.MfaRequired,
                src.DomainsAllowed));
    }
}
