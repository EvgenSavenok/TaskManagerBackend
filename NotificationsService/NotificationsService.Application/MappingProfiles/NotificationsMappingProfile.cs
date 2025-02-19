using AutoMapper;
using NotificationsService.Application.DataTransferObjects.NotificationsDto;
using NotificationsService.Application.UseCases.Commands.NotificationCommands.CreateNotification;
using NotificationsService.Application.UseCases.Commands.NotificationCommands.UpdateNotification;
using NotificationsService.Domain.Models;

namespace NotificationsService.Application.MappingProfiles;

public class NotificationsMappingProfile : Profile
{
    public NotificationsMappingProfile()
    {
        CreateMap<CreateNotificationCommand, Notification>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.NotificationDto.Title))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.NotificationDto.UserId))
            .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.NotificationDto.TaskId))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.NotificationDto.Deadline))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.NotificationDto.Status))
            .ForMember(dest => dest.MinutesBeforeDeadline, opt => opt.MapFrom(src => src.NotificationDto.MinutesBeforeDeadline))
            .ForMember(dest => dest.UserTimeZone, opt => opt.MapFrom(src => src.NotificationDto.UserTimeZone));
    
        CreateMap<Notification, NotificationDto>();

        CreateMap<UpdateNotificationCommand, Notification>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.NotificationDto.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.NotificationDto.UserId))
            .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.NotificationDto.TaskId))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.NotificationDto.Title))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.NotificationDto.Status))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.NotificationDto.CreatedAt))
            .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.NotificationDto.Deadline))
            .ForMember(dest => dest.MinutesBeforeDeadline, opt => opt.MapFrom(src => src.NotificationDto.MinutesBeforeDeadline))
            .ForMember(dest => dest.UserTimeZone, opt => opt.MapFrom(src => src.NotificationDto.UserTimeZone));
    }
}