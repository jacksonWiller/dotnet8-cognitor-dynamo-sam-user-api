using AutoMapper;
using Catalog.Domain.Entities.BookAggregate;
using Catalog.Domain.ValueObjects;
using Arda9UserApi.Application.DTOs;

namespace Arda9UserApi.Application.Mappings;

/// <summary>
/// Perfil de mapeamento do AutoMapper entre entidades de domínio e DTOs
/// </summary>
public class BookMappingProfile : Profile
{
    public BookMappingProfile()
    {
        // Mapeamento bidirecional de Book (Domain) para BookDto (usado para API e persistência)
        CreateMap<Book, BookDto>()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src._isDeleted))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest._isDeleted, opt => opt.MapFrom(src => src.IsDeleted));

        // Mapeamento bidirecional de Category (Domain) para CategoryDto
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src._isDeleted))
            .ReverseMap()
            .ForMember(dest => dest._isDeleted, opt => opt.MapFrom(src => src.IsDeleted))
            .ForMember(dest => dest.Books, opt => opt.Ignore()); // Evita referência circular

        // Mapeamento bidirecional de Image (Domain) para ImageDto
        CreateMap<Image, ImageDto>()
            .ReverseMap();

        // Mapeamento bidirecional de Tag (Domain) para TagDto
        CreateMap<Tag, TagDto>()
            .ReverseMap();
    }
}
