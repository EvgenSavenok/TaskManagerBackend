using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TagsDto;
using Application.UseCases.Queries.TagQueries.GetAllTags;
using AutoMapper;
using Moq;
using TasksService.Domain.Models;

namespace ModuleTests.TasksService.Tags.Queries;

public class GetAllTagsQueryHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRedisCacheService> _cacheMock = new();
    private readonly GetAllTagsQueryHandler _handler;

    public GetAllTagsQueryHandlerTests()
    {
        _handler = new GetAllTagsQueryHandler(
            _repositoryMock.Object,
            _mapperMock.Object,
            _cacheMock.Object
        );
    }

    [Fact]
    public async Task Handle_ReturnsCachedTags_WhenAvailable()
    {
        var cachedTags = new List<TagDto> { new() { Id = Guid.NewGuid(), TagName = "cached" } };
        _cacheMock.Setup(c => c.GetAsync<IEnumerable<TagDto>>("tags: all")).ReturnsAsync(cachedTags);

        var result = await _handler.Handle(new GetAllTagsQuery(), CancellationToken.None);

        Assert.Equal(cachedTags, result);
        _repositoryMock.Verify(r => r.Tag.FindAll(false, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsTagsFromDbAndSetsCache_WhenCacheIsEmpty()
    {
        _cacheMock.Setup(c => c.GetAsync<IEnumerable<TagDto>>("tags: all")).ReturnsAsync((IEnumerable<TagDto>)null!);

        var dbTags = new List<Tag> { new() { Id = Guid.NewGuid(), Name = "db" } };
        var tagDtos = new List<TagDto> { new() { Id = dbTags[0].Id, TagName = "db" } };

        _repositoryMock.Setup(r => r.Tag.FindAll(false, It.IsAny<CancellationToken>())).ReturnsAsync(dbTags);
        _mapperMock.Setup(m => m.Map<IEnumerable<TagDto>>(dbTags)).Returns(tagDtos);

        var result = await _handler.Handle(new GetAllTagsQuery(), CancellationToken.None);

        Assert.Equal(tagDtos, result);
        _cacheMock.Verify(c => c.SetAsync<IEnumerable<TagDto>>(
                "tags: all",
                It.IsAny<IEnumerable<TagDto>>(),
                It.IsAny<TimeSpan>()),
            Times.Once);

    }
}