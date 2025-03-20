using Application.Contracts.MessagingContracts;
using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using MediatR;
using TasksService.Domain.CustomExceptions;

namespace Application.UseCases.Commands.TaskCommands.DeleteTask;

public class DeleteTaskCommandHandler(
    IRepositoryManager repository,
    ITaskDeletedProducer taskDeletedProducer,
    IRedisCacheService cache)
    : IRequestHandler<DeleteTaskCommand>
{
    public async Task<Unit> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var taskId = request.TaskId;
        
        var taskEntity = await repository.Task.GetTaskByIdAsync(taskId, trackChanges: true, cancellationToken);
        if (taskEntity == null)
        {
            throw new NotFoundException($"Task with id {taskId} not found.");
        }

        await repository.Task.Delete(taskEntity, cancellationToken);
        
        taskDeletedProducer.PublishTaskDeletedEvent(taskId);
        
        string cacheKey = $"tasks:user:{taskEntity.UserId}";
        await cache.RemoveAsync(cacheKey);
        
        return Unit.Value;
    }
}