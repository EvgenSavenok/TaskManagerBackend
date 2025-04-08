using AutoMapper;
using FluentAssertions;
using Moq;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Application.DataTransferObjects.NotificationsDto;
using NotificationsService.Application.UseCases.Queries.NotificationQueries.GetAllNotifications;
using NotificationsService.Domain.Enums;
using NotificationsService.Domain.Models;

namespace Tests.NotificationsService.Queries;

public class GetAllNotificationsQueryHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllNotificationsQueryHandler _handler;

    public GetAllNotificationsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IRepositoryManager>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllNotificationsQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsMappedNotifications()
    {
        // Arrange
        var notifications = new List<Notification>
        {
            new Notification
            {
                Id = Guid.NewGuid(),
                Title = "Test",
                CreatedAt = DateTime.UtcNow,
                Deadline = DateTime.UtcNow.AddDays(1),
                MinutesBeforeDeadline = 10,
                UserEmail = "user@mail.com",
                UserTimeZone = "UTC",
                HangfireJobId = "job1",
                TaskId = Guid.NewGuid(),
                Status = Status.Unsent
            }
        };

        var dtoList = new List<NotificationDto>
        {
            new NotificationDto
            {
                Id = notifications[0].Id,
                Title = "Test",
                CreatedAt = notifications[0].CreatedAt,
                Deadline = notifications[0].Deadline,
                MinutesBeforeDeadline = 10,
                UserEmail = "user@mail.com",
                UserTimeZone = "UTC",
                HangfireJobId = "job1",
                TaskId = notifications[0].TaskId
            }
        };

        _repositoryMock.Setup(r => r.Notification.FindAll(It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        _mapperMock.Setup(m => m.Map<IEnumerable<NotificationDto>>(notifications))
            .Returns(dtoList);

        // Act
        var result = await _handler.Handle(new GetAllNotificationsQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Test");
    }
}