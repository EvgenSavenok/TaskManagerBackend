using Application.DataTransferObjects.TasksDto;
using AutoMapper;
using TasksService.Domain.Models;

namespace Application.MappingProfiles;

public class TasksMappingProfile : Profile
{
    public TasksMappingProfile()
    {
        //Is it necessary to make all mappings?
        CreateMap<CustomTask, TaskDto>().ReverseMap();
        
        CreateMap<TaskForCreationDto, CustomTask>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid())) 
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow)) 
            .ForMember(dest => dest.TaskTags, opt => opt.Ignore()) 
            .ForMember(dest => dest.TaskComments, opt => opt.Ignore());
        }
}