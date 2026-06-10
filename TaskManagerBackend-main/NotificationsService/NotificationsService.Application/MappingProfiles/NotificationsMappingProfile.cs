using AutoMapper;
using NotificationsService.Application.DataTransferObjects.NotificationsDto;
using NotificationsService.Application.DataTransferObjects.TaskEventDto;
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
            .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.NotificationDto.TaskId))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.NotificationDto.Title))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.NotificationDto.Deadline))
            .ForMember(dest => dest.MinutesBeforeDeadline, opt => opt.MapFrom(src => src.NotificationDto.MinutesBeforeDeadline))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.NotificationDto.UserEmail))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 1))
            .ForMember(dest => dest.UserTimeZone, opt => opt.MapFrom(src => src.NotificationDto.UserTimeZone));
    
        CreateMap<Notification, NotificationDto>();
        
        CreateMap<NotificationDto, Notification>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => 2));

        CreateMap<UpdateNotificationCommand, Notification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.NotificationDto.Title))
            .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.NotificationDto.Deadline))
            .ForMember(dest => dest.MinutesBeforeDeadline, opt => opt.MapFrom(src => src.NotificationDto.MinutesBeforeDeadline))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 1))
            .ForMember(dest => dest.UserTimeZone, opt => opt.MapFrom(src => src.NotificationDto.UserTimeZone));

        CreateMap<CreateTaskEventDto, NotificationDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        
        CreateMap<UpdateTaskEventDto, NotificationDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TaskId));
    }
}