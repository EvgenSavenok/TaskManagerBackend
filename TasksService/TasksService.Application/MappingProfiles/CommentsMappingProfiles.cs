using Application.DataTransferObjects.CommentsDto;
using Application.UseCases.Commands.CommentCommands.CreateComment;
using AutoMapper;
using TasksService.Domain.Models;

namespace Application.MappingProfiles;

public class CommentsMappingProfiles : Profile
{
    public CommentsMappingProfiles()
    {
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content));
    }
}