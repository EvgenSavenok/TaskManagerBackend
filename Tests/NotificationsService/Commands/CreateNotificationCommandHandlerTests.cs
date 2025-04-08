using AutoMapper;
using FluentValidation;
using MediatR;
using Moq;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Application.Contracts.ServicesContracts;
using NotificationsService.Application.DataTransferObjects.NotificationsDto;
using NotificationsService.Application.UseCases.Commands.NotificationCommands.CreateNotification;
using NotificationsService.Domain.Enums;
using NotificationsService.Domain.Models;

namespace Tests.NotificationsService.Commands;

public class CreateNotificationCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock;
    private readonly Mock<IValidator<Notification>> _validatorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IHangfireService> _hangfireServiceMock;
    private readonly CreateNotificationCommandHandler _handler;

    public CreateNotificationCommandHandlerTests()
    {
        _repositoryMock = new Mock<IRepositoryManager>();
        _validatorMock = new Mock<IValidator<Notification>>();
        _mapperMock = new Mock<IMapper>();
        _hangfireServiceMock = new Mock<IHangfireService>();
        _handler = new CreateNotificationCommandHandler(
            _repositoryMock.Object, 
            _validatorMock.Object, 
            _mapperMock.Object, 
            _hangfireServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateNotification_WhenValidRequest()
    {
        // Arrange
        var notificationDto = new NotificationDto
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            Title = "Test Notification",
            CreatedAt = DateTime.UtcNow,
            Deadline = DateTime.UtcNow.AddDays(1),
            MinutesBeforeDeadline = 30,
            UserTimeZone = "UTC",
            UserEmail = "user@example.com"
        };

        var request = new CreateNotificationCommand
        {
            NotificationDto = notificationDto
        };

        var notificationEntity = new Notification
        {
            Id = notificationDto.Id,
            TaskId = notificationDto.TaskId,
            Title = notificationDto.Title,
            CreatedAt = notificationDto.CreatedAt,
            Deadline = notificationDto.Deadline,
            MinutesBeforeDeadline = notificationDto.MinutesBeforeDeadline,
            UserTimeZone = notificationDto.UserTimeZone,
            UserEmail = notificationDto.UserEmail,
            Status = Status.Sleep,
            HangfireJobId = string.Empty
        };

        _mapperMock.Setup(m => m.Map<Notification>(It.IsAny<CreateNotificationCommand>()))
            .Returns(notificationEntity);

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _hangfireServiceMock.Setup(h => h.ScheduleNotificationInHangfire(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .Returns("jobId");
        
        _repositoryMock.Setup(r => r.Notification.Create(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask); 

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        _repositoryMock.Verify(r => r.Notification.Create(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Once);
        _hangfireServiceMock.Verify(h => h.ScheduleNotificationInHangfire(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenInvalidNotification()
    {
        // Arrange
        var notificationDto = new NotificationDto
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            Title = "Test Notification",
            CreatedAt = DateTime.UtcNow,
            Deadline = DateTime.UtcNow.AddDays(1),
            MinutesBeforeDeadline = 30,
            UserTimeZone = "UTC",
            UserEmail = "user@example.com"
        };

        var request = new CreateNotificationCommand
        {
            NotificationDto = notificationDto
        };

        var notificationEntity = new Notification
        {
            Id = notificationDto.Id,
            TaskId = notificationDto.TaskId,
            Title = notificationDto.Title,
            CreatedAt = notificationDto.CreatedAt,
            Deadline = notificationDto.Deadline,
            MinutesBeforeDeadline = notificationDto.MinutesBeforeDeadline,
            UserTimeZone = notificationDto.UserTimeZone,
            UserEmail = notificationDto.UserEmail,
            Status = Status.Sleep,
            HangfireJobId = string.Empty
        };

        _mapperMock.Setup(m => m.Map<Notification>(It.IsAny<CreateNotificationCommand>()))
            .Returns(notificationEntity);

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult
            {
                Errors = { new FluentValidation.Results.ValidationFailure("Title", "Title is required") }
            });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Contains("Title is required", exception.Message);
    }
}