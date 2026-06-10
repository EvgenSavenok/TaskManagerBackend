using Application.DataTransferObjects.CategoriesDto;
using Application.UseCases.Commands.CategoryCommands.CreateCategory;
using Application.UseCases.Commands.CategoryCommands.UpdateCategory;
using AutoMapper;
using TasksService.Domain.Models;

namespace Application.MappingProfiles;

public class CategoriesMappingProfile : Profile
{
    public CategoriesMappingProfile()
    {
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Name));

        CreateMap<CreateCategoryCommand, Category>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CategoryName));

        CreateMap<UpdateCategoryCommand, Category>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CategoryName));
    }
}
