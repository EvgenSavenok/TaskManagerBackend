using Application.DataTransferObjects.TagsDto;
using Application.UseCases.Commands.TagCommands.CreateTag;
using Application.UseCases.Commands.TagCommands.UpdateTag;
using AutoMapper;
using TasksService.Domain.Models;

namespace Application.MappingProfiles;

public class TagsMappingProfiles : Profile
{
    public TagsMappingProfiles()
    {
        CreateMap<Tag, TagDto>()
            .ForMember(dest => dest.TagName, opt => opt.MapFrom(src => src.Name));
        
        CreateMap<CreateTagCommand, Tag>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.TagName));

        CreateMap<UpdateTagCommand, Tag>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TagId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.TagName));
    }
}