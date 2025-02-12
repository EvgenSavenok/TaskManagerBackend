using Application.DataTransferObjects.CommentsDto;
using Application.DataTransferObjects.TasksDto;
using Application.UseCases.Commands.TaskCommands.CreateTask;
using Application.UseCases.Commands.TaskCommands.UpdateTask;
using AutoMapper;
using TasksService.Domain.Models;

namespace Application.MappingProfiles;

public class TasksMappingProfile : Profile
{
    public TasksMappingProfile()
    {
        CreateMap<TaskDto, CustomTask>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ReverseMap();
        
        CreateMap<CreateTaskCommand, CustomTask>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.TaskDto.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.TaskDto.Description))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.TaskDto.Category))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.TaskDto.Priority))
            .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.TaskDto.Deadline))
            .ForMember(dest => dest.TaskTags, opt => opt.MapFrom(src =>
                src.TaskDto.TaskTags.Select(tagDto => new Tag
                {
                    Id = tagDto.Id,
                    Name = tagDto.TagName
                }).ToList()))
            .ForMember(dest => dest.TaskComments, opt => opt.MapFrom(src =>
                src.TaskDto.TaskComments.Select(commentDto => new Comment
                {
                    Id = commentDto.Id,
                    Content = commentDto.Content, 
                }).ToList()));
        
        CreateMap<UpdateTaskCommand, CustomTask>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TaskDto.Id))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.TaskDto.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.TaskDto.Description))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.TaskDto.Category))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.TaskDto.Priority))
            .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.TaskDto.Deadline))
            .ForMember(dest => dest.TaskTags, opt => opt.MapFrom(src => 
                src.TaskDto.TaskTags.Select(tagDto => new Tag 
                { 
                    Id = tagDto.Id,
                    Name = tagDto.TagName 
                }).ToList()))
            .ForMember(dest => dest.TaskComments, opt => opt.MapFrom(src => src.TaskDto.TaskComments));
    }
}