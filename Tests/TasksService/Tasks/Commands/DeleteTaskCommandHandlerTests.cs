using Application.Contracts.MessagingContracts;
using Application.Contracts.RepositoryContracts;
using Application.UseCases.Commands.TaskCommands.DeleteTask;
using MediatR;
using Moq;
using TasksService.Domain.CustomExceptions;
using TasksService.Domain.Models;

namespace Tests.TasksService.Tasks.Commands;

public class DeleteTaskCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock = new();
    private readonly Mock<ITaskDeletedProducer> _producerMock = new();

    private readonly DeleteTaskCommandHandler _handler;

    public DeleteTaskCommandHandlerTests()
    {
        _handler = new DeleteTaskCommandHandler(
            _repositoryMock.Object,
            _producerMock.Object
        );
    }
    
    [Fact]
    public async Task Handle_ShouldDeleteTask_WhenTaskExists()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new CustomTask { Id = taskId };

        _repositoryMock.Setup(r => r.Task.GetTaskByIdAsync(taskId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _repositoryMock.Setup(r => r.Task.Delete(task, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(new DeleteTaskCommand { TaskId = taskId }, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.Task.Delete(task, It.IsAny<CancellationToken>()), Times.Once);
        _producerMock.Verify(p => p.PublishTaskDeletedEvent(taskId), Times.Once);
        Assert.Equal(Unit.Value, result);
    }
    
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.Task.GetTaskByIdAsync(taskId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomTask)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new DeleteTaskCommand { TaskId = taskId }, CancellationToken.None));
    }
}