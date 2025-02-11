using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TasksDto;
using AutoMapper;
using FluentValidation;
using MediatR;
using TasksService.Domain.CustomExceptions;
using TasksService.Domain.Models;

namespace Application.UseCases.Commands.TagCommands.CreateTag;

public class CreateTagCommandHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IValidator<Tag> validator)
    : IRequestHandler<CreateTagCommand>
{
    public async Task<Unit> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tagEntity = mapper.Map<Tag>(request);
        var validationResult = await validator.ValidateAsync(tagEntity, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        Guid taskId = request.TaskId;
        var task = await repository.Task.GetTaskByIdAsync(taskId, trackChanges: false, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException($"Task with id {taskId} not found.");
        }
        
        var tag = await repository.Tag.GetTagByName(
            taskId, 
            tagEntity.Name,
            trackChanges: false, 
            cancellationToken);

        if (tag != null)
        {
            throw new AlreadyExistsException($"Tag with name {tagEntity.Name} already exists.");
        }

        await repository.Tag.Create(tagEntity, cancellationToken);

        task.TaskTags.Add(tagEntity);
        await repository.Task.Update(task, cancellationToken);
        
        return Unit.Value;
    }
}