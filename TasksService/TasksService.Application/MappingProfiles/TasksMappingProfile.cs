using Application.DataTransferObjects.TasksDto;
using Application.UseCases.Commands.TaskCommands;
using AutoMapper;
using TasksService.Domain.Models;

namespace Application.MappingProfiles;

public class TasksMappingProfile : Profile
{
    public TasksMappingProfile()
    {
        CreateMap<CustomTask, TaskDto>().ReverseMap();
        
        CreateMap<CreateTaskCommand, CustomTask>();
        
        CreateMap<TaskForCreationDto, CustomTask>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid())) 
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow)) 
            .ForMember(dest => dest.TaskTags, opt => opt.Ignore()) 
            .ForMember(dest => dest.TaskComments, opt => opt.Ignore());
        }
}