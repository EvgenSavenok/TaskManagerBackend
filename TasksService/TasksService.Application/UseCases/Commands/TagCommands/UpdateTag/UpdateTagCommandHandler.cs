using Application.Contracts.RepositoryContracts;
using AutoMapper;
using FluentValidation;
using MediatR;
using TasksService.Domain.CustomExceptions;
using TasksService.Domain.Models;

namespace Application.UseCases.Commands.TagCommands.UpdateTag;

public class UpdateTagCommandHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IValidator<Tag> validator)
    : IRequestHandler<UpdateTagCommand>
{
    public async Task<Unit> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var taskId = request.TaskId;
        var tagId = request.TagId;
        
        var tagEntity = await repository.Tag.GetTagByTaskId(
            taskId, 
            tagId, 
            trackChanges: true, 
            cancellationToken);
        if (tagEntity == null)
        {
            throw new NotFoundException($"Tag with id {tagId} not found.");
        }
        
        mapper.Map(request, tagEntity);
        
        var validationResult = await validator.ValidateAsync(tagEntity, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        await repository.Tag.Update(tagEntity, cancellationToken);
        
        return Unit.Value;
    }
}