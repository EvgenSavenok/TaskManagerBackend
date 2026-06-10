using Application.DataTransferObjects.CommentsDto;
using Application.UseCases.Commands.CommentCommands.CreateComment;
using Application.UseCases.Commands.CommentCommands.UpdateComment;
using AutoMapper;
using TasksService.Domain.Models;

namespace Application.MappingProfiles;

public class CommentsMappingProfiles : Profile
{
    public CommentsMappingProfiles()
    {
        CreateMap<CreateCommentCommand, Comment>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<UpdateCommentCommand, Comment>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CommentId));

        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.CommentId, opt => opt.MapFrom(src => src.Id));
    }
}