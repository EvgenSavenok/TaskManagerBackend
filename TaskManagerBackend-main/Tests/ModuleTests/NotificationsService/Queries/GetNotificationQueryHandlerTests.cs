using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Application.DataTransferObjects.NotificationsDto;
using NotificationsService.Application.UseCases.Queries.NotificationQueries.GetNotificationById;
using NotificationsService.Domain.CustomExceptions;
using NotificationsService.Domain.Models;

namespace ModuleTests.NotificationsService.Queries;

public class GetNotificationQueryHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetNotificationQueryHandler _handler;

    public GetNotificationQueryHandlerTests()
    {
        _repositoryMock = new Mock<IRepositoryManager>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetNotificationQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidNotification_ReturnsNotificationDto()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        var notification = new Notification
        {
            Id = notificationId,
            Title = "Test Notification",
            Deadline = DateTime.UtcNow
        };

        var notificationDto = new NotificationDto
        {
            Id = notificationId,
            Title = "Test Notification",
            Deadline = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.Notification.FindByCondition(It.IsAny<Expression<Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification> { notification });

        _mapperMock.Setup(m => m.Map<NotificationDto>(It.IsAny<Notification>())).Returns(notificationDto);

        var query = new GetNotificationQuery { NotificationId = notificationId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(notificationDto);
    }

    [Fact]
    public async Task Handle_NotificationNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.Notification.FindByCondition(It.IsAny<Expression<Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());

        var query = new GetNotificationQuery { NotificationId = notificationId };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
    }
}