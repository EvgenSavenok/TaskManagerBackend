using Application.Contracts.RepositoryContracts;
using AutoMapper;
using FluentValidation;
using MediatR;
using TasksService.Domain.CustomExceptions;
using TasksService.Domain.Models;

namespace Application.UseCases.Commands.TaskCommands.UpdateTask;

public class UpdateTaskCommandHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IValidator<CustomTask> validator)
    : IRequestHandler<UpdateTaskCommand>
{
    public async Task<Unit> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var taskId = request.TaskDto.Id;
    
        var taskEntity = await repository.Task.GetTaskByIdAsync(taskId, trackChanges: true, cancellationToken);
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
                 trackChanges: false, 
                 cancellationToken))
             .ToList();
        
        foreach (var tag in tagsToAdd)
        {
            repository.Tag.Attach(tag);
        }

        taskEntity.TaskTags = tagsToAdd;
        
        await repository.Task.Update(taskEntity, cancellationToken);
    
        return Unit.Value;
    }
}