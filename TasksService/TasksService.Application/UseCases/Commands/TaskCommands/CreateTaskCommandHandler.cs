using Application.Contracts.RepositoryContracts;
using AutoMapper;
using FluentValidation;
using MediatR;
using TasksService.Domain.Models;

namespace Application.UseCases.Commands.TaskCommands;

public class CreateTaskCommandHandler(
    IRepositoryManager repository,
    IMapper mapper, 
    IValidator<CustomTask> validator)
    : IRequestHandler<CreateTaskCommand>
{
    public async Task<Unit> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var taskEntity = mapper.Map<CustomTask>(request);
        var validationResult = await validator.ValidateAsync(taskEntity);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        repository.Task.Create(taskEntity);
        await repository.SaveAsync();
        
        return Unit.Value; 
    }
}