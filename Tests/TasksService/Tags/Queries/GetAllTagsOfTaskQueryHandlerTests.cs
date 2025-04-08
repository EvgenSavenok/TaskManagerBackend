using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TagsDto;
using Application.UseCases.Queries.TagQueries.GetAllTagsOfTask;
using AutoMapper;
using Moq;
using TasksService.Domain.Models;

namespace Tests.TasksService.Tags.Queries;

public class GetAllTagsOfTaskQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsMappedTags()
    {
        var taskId = Guid.NewGuid();
        var tags = new List<Tag> { new() { Id = Guid.NewGuid(), Name = "important" } };
        var tagDtos = new List<TagDto> { new() { Id = tags[0].Id, TagName = "important" } };

        var repositoryMock = new Mock<IRepositoryManager>();
        var mapperMock = new Mock<IMapper>();

        repositoryMock.Setup(r => r.Tag.FindByCondition(
                It.IsAny<System.Linq.Expressions.Expression<Func<Tag, bool>>>(), 
                false, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tags);

        mapperMock.Setup(m => m.Map<IEnumerable<TagDto>>(tags)).Returns(tagDtos);

        var handler = new GetAllTagsOfTaskQueryHandler(repositoryMock.Object, mapperMock.Object);
        var query = new GetAllTagsOfTaskQuery { TaskId = taskId };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("important", result.First().TagName);
    }
}