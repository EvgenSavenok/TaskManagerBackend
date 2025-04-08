using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Application.Contracts.ServicesContracts;
using NotificationsService.Application.DataTransferObjects.NotificationsDto;
using NotificationsService.Application.UseCases.Commands.NotificationCommands.UpdateNotification;
using NotificationsService.Domain.Enums;
using NotificationsService.Domain.Models;

namespace Tests.NotificationsService.Commands;

public class UpdateNotificationCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock;
    private readonly Mock<IValidator<Notification>> _validatorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IHangfireService> _hangfireServiceMock;
    private readonly UpdateNotificationCommandHandler _handler;

    public UpdateNotificationCommandHandlerTests()
    {
        _repositoryMock = new Mock<IRepositoryManager>();
        _validatorMock = new Mock<IValidator<Notification>>();
        _mapperMock = new Mock<IMapper>();
        _hangfireServiceMock = new Mock<IHangfireService>();
        _handler = new UpdateNotificationCommandHandler(
            _repositoryMock.Object,
            _validatorMock.Object,
            _mapperMock.Object,
            _hangfireServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesNotificationAndSchedulesHangfireJob()
    {
        // Arrange
        var notificationDto = new NotificationDto
        {
            TaskId = Guid.NewGuid(),
            Title = "Updated Notification",
            Deadline = DateTime.UtcNow.AddHours(1)
        };

        var existingNotification = new Notification
        {
            TaskId = notificationDto.TaskId,
            Title = "Old Notification",
            Deadline = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.Notification.FindByCondition(It.IsAny<Expression<Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification> { existingNotification });

        _mapperMock.Setup(m => m.Map(It.IsAny<UpdateNotificationCommand>(), It.IsAny<Notification>())).Callback<UpdateNotificationCommand, Notification>((cmd, notif) => 
        {
            notif.Title = cmd.NotificationDto.Title;
            notif.Deadline = cmd.NotificationDto.Deadline;
        });

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var command = new UpdateNotificationCommand
        {
            NotificationDto = notificationDto,
            Status = Status.Sleep
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.Notification.UpdateNotificationByTaskId(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Once);
        _hangfireServiceMock.Verify(h => h.ScheduleNotificationInHangfire(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidUpdate_ThrowsValidationException()
    {
        // Arrange
        var notificationDto = new NotificationDto
        {
            TaskId = Guid.NewGuid(),
            Title = "Updated Notification"
        };

        var existingNotification = new Notification
        {
            TaskId = notificationDto.TaskId,
            Title = "Old Notification",
            Deadline = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.Notification.FindByCondition(It.IsAny<Expression<Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification> { existingNotification });

        _mapperMock.Setup(m => m.Map(It.IsAny<UpdateNotificationCommand>(), It.IsAny<Notification>())).Callback<UpdateNotificationCommand, Notification>((cmd, notif) => 
        {
            notif.Title = cmd.NotificationDto.Title;
        });

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Title", "Title cannot be empty") }));

        var command = new UpdateNotificationCommand
        {
            NotificationDto = notificationDto,
            Status = Status.Sleep
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}