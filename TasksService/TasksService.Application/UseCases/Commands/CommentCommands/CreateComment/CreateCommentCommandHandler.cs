using Application.Contracts.RepositoryContracts;
using AutoMapper;
using FluentValidation;
using MediatR;
using TasksService.Domain.CustomExceptions;
using TasksService.Domain.Models;

namespace Application.UseCases.Commands.CommentCommands.CreateComment;

public class CreateCommentCommandHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IValidator<Comment> validator)
    : IRequestHandler<CreateCommentCommand>
{
    public async Task<Unit> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var commentEntity = mapper.Map<Comment>(request);
        var validationResult = await validator.ValidateAsync(commentEntity, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var taskEntity = await repository.Task.GetTaskByIdAsync(
            request.TaskId,
            trackChanges: false,
            cancellationToken);
        if (taskEntity == null)
        {
            throw new NotFoundException($"Task with ID {request.TaskId} not found.");
        }
        
        commentEntity.TaskId = taskEntity.Id;
        
        await repository.Comment.Create(commentEntity, cancellationToken);

        return Unit.Value;
    }
}