using Application.Contracts.RepositoryContracts;
using AutoMapper;
using FluentValidation;
using MediatR;
using TasksService.Domain.Models;

namespace Application.UseCases.Commands.TaskCommands.CreateTask;

public class CreateTaskCommandHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IValidator<CustomTask> validator)
    : IRequestHandler<CreateTaskCommand>
{
    public async Task<Unit> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var taskEntity = mapper.Map<CustomTask>(request);
        
        var validationResult = await validator.ValidateAsync(taskEntity, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var tagNames = taskEntity.TaskTags.Select(tag => tag.Name).ToList();
        var existingTags = (await repository.Tag.FindByCondition(
                tag => tagNames.Contains(tag.Name), 
                trackChanges: true, 
                cancellationToken))
            .ToList();
        
        taskEntity.TaskTags = existingTags;
        await repository.Task.Create(taskEntity, cancellationToken);
        
        return Unit.Value; 
    }
}