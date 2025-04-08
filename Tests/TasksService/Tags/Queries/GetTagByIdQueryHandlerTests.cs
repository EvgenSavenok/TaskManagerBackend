using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TagsDto;
using Application.UseCases.Queries.TagQueries.GetTagByTaskId;
using AutoMapper;
using Moq;
using TasksService.Domain.CustomExceptions;
using TasksService.Domain.Models;

namespace Tests.TasksService.Tags.Queries;

public class GetTagByIdQueryHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRedisCacheService> _cacheMock = new();
    private readonly GetTagByIdQueryHandler _handler;

    public GetTagByIdQueryHandlerTests()
    {
        _handler = new GetTagByIdQueryHandler(
            _repositoryMock.Object,
            _mapperMock.Object,
            _cacheMock.Object
        );
    }

    [Fact]
    public async Task Handle_ReturnsTagFromCache_WhenAvailable()
    {
        var tagId = Guid.NewGuid();
        var cachedTag = new TagDto { Id = tagId, TagName = "cached" };
        _cacheMock.Setup(c => c.GetAsync<TagDto>($"tag: {tagId}")).ReturnsAsync(cachedTag);

        var result = await _handler.Handle(new GetTagByIdQuery { TagId = tagId }, CancellationToken.None);

        Assert.Equal("cached", result.TagName);
        _repositoryMock.Verify(r => r.Tag.GetTagById(It.IsAny<Guid>(), false, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenTagNotFound()
    {
        var tagId = Guid.NewGuid();
        _cacheMock.Setup(c => c.GetAsync<TagDto>($"tag: {tagId}")).ReturnsAsync((TagDto)null!);
        _repositoryMock.Setup(r => r.Tag.GetTagById(tagId, false, It.IsAny<CancellationToken>())).ReturnsAsync((Tag)null!);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new GetTagByIdQuery { TagId = tagId }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ReturnsTagFromDbAndSetsCache_WhenCacheIsEmpty()
    {
        var tagId = Guid.NewGuid();
        var tag = new Tag { Id = tagId, Name = "urgent" };
        var tagDto = new TagDto { Id = tagId, TagName = "urgent" };

        _cacheMock.Setup(c => c.GetAsync<TagDto>($"tag: {tagId}")).ReturnsAsync((TagDto)null!);
        _repositoryMock.Setup(r => r.Tag.GetTagById(tagId, false, It.IsAny<CancellationToken>())).ReturnsAsync(tag);
        _mapperMock.Setup(m => m.Map<TagDto>(tag)).Returns(tagDto);

        var result = await _handler.Handle(new GetTagByIdQuery { TagId = tagId }, CancellationToken.None);

        Assert.Equal("urgent", result.TagName);
        _cacheMock.Verify(c => c.SetAsync($"tag: {tagId}", tagDto, It.IsAny<TimeSpan>()), Times.Once);
    }
}