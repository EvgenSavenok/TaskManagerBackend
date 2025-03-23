using Application.Contracts.Grpc;
using Application.Contracts.MessagingContracts;
using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TasksDto;
using AutoMapper;
using FluentValidation;
using MediatR;
using TasksService.Domain.CustomExceptions;
using TasksService.Domain.Models;

namespace Application.UseCases.Commands.TaskCommands.UpdateTask;

public class UpdateTaskCommandHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IValidator<CustomTask> validator,
    ITaskUpdatedProducer taskUpdatedProducer,
    IUserGrpcService userGrpcService,
    IRedisCacheService cache)
    : IRequestHandler<UpdateTaskCommand>
{
    public async Task<Unit> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var taskId = request.TaskDto.TaskId;
    
        var taskEntity = await repository.Task.GetTaskByIdAsync(
            taskId, 
            trackChanges: true, 
            cancellationToken);
        if (taskEntity == null)
        {
            throw new NotFoundException($"Task with id {taskId} not found.");
        }
    
        mapper.Map(request.TaskDto, taskEntity);
    
        var validationResult = await validator.ValidateAsync(taskEntity, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var tagNamesInRequest = request.TaskDto.TaskTags.Select(tag => tag.TagName).ToList();
        
        var tagsToAdd = (await repository.Tag.FindByCondition(
                 tag => tagNamesInRequest.Contains(tag.Name), 
                 trackChanges: true, 
                 cancellationToken))
             .ToList();

        taskEntity.TaskTags = tagsToAdd;
        
        await repository.Task.Update(taskEntity, cancellationToken);

        var taskEventDto = new UpdateTaskEventDto();
        mapper.Map(request.TaskDto, taskEventDto);
        
        var userEmail = await userGrpcService.GetUserEmailAsync(request.TaskDto.UserId);
        taskEventDto.UserEmail = userEmail;
        
        taskUpdatedProducer.PublishTaskUpdatedEvent(taskEventDto);
    
        string cacheKey = $"tasks:user:{taskEntity.UserId}";
        await cache.RemoveAsync(cacheKey);
        
        return Unit.Value;
    }
}