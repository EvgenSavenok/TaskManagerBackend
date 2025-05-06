using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TagsDto;
using AutoMapper;
using FluentValidation;
using MediatR;
using TasksService.Domain.CustomExceptions;
using TasksService.Domain.Models;

namespace Application.UseCases.Commands.TagCommands.CreateTag;

public class CreateTagCommandHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IRedisCacheService cache,
    IValidator<Tag> validator)
    : IRequestHandler<CreateTagCommand, Tag>
{
    public async Task<Tag> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tagEntity = mapper.Map<Tag>(request);
        var validationResult = await validator.ValidateAsync(tagEntity, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var tag = await repository.Tag.GetTagByName(
            tagEntity.Name,
            trackChanges: false, 
            cancellationToken);

        if (tag != null)
        {
            throw new AlreadyExistsException($"Tag with name {tagEntity.Name} already exists.");
        }

        await repository.Tag.Create(tagEntity, cancellationToken);
        
        string cacheKey = "tags: all";
        await cache.RemoveAsync(cacheKey);
        
        return tagEntity;
    }
}