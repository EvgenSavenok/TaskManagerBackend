using System.Linq.Expressions;
using Moq;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Application.Contracts.ServicesContracts;
using NotificationsService.Application.UseCases.Commands.NotificationCommands.DeleteNotification;
using NotificationsService.Domain.Models;

namespace ModuleTests.NotificationsService.Commands;

public class DeleteNotificationCommandHandlerTests
{
     private readonly Mock<IRepositoryManager> _repositoryMock;
    private readonly Mock<IHangfireService> _hangfireServiceMock;
    private readonly DeleteNotificationCommandHandler _handler;

    public DeleteNotificationCommandHandlerTests()
    {
        _repositoryMock = new Mock<IRepositoryManager>();
        _hangfireServiceMock = new Mock<IHangfireService>();
        _handler = new DeleteNotificationCommandHandler(_repositoryMock.Object, _hangfireServiceMock.Object);
    }

    [Fact]
    public async Task Handle_NotificationExists_DeletesNotificationAndHangfireJob()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var notification = new Notification
        {
            TaskId = taskId,
            HangfireJobId = "jobId123"
        };

        _repositoryMock.Setup(r => r.Notification.FindByCondition(It.IsAny<Expression<Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification> { notification });

        var command = new DeleteNotificationCommand { Id = taskId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.Notification.Delete(It.IsAny<Expression<Func<Notification, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _hangfireServiceMock.Verify(h => h.DeleteNotificationInHangfire("jobId123"), Times.Once);
    }

    [Fact]
    public async Task Handle_NotificationNotFound_DoesNotDeleteAnything()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.Notification.FindByCondition(It.IsAny<Expression<Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());

        var command = new DeleteNotificationCommand { Id = taskId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.Notification.Delete(It.IsAny<Expression<Func<Notification, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _hangfireServiceMock.Verify(h => h.DeleteNotificationInHangfire(It.IsAny<string>()), Times.Never);
    }
}