using System.Net;
using System.Net.Http.Json;
using Application.DataTransferObjects.TasksDto;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TasksService.Domain.Enums;

namespace IntegrationTests;

public class TasksTests : IClassFixture<TasksServiceApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JwtTokenGenerator _tokenGenerator;
    
    public TasksTests(TasksServiceApplicationFactory factory)
    {
        var jwtSettings = factory.Services.GetRequiredService<IConfiguration>().GetSection("JwtSettings");
        var secretKey = jwtSettings["validIssuer"];
        var issuer = jwtSettings["ValidIssuer"];
        var audience = jwtSettings["ValidAudience"];
        
        _tokenGenerator = new JwtTokenGenerator(secretKey, issuer, audience);

        _client = factory.CreateClient();
        
        var token = _tokenGenerator.GenerateToken(Guid.NewGuid().ToString());
        
        _client.DefaultRequestHeaders.Authorization 
            = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
    
    [Fact]
    public async Task CreateTask_ShouldReturnNoContent_AndPublishEvent()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "Integration test task",
            Description = "Testing CreateTask handler",
            Deadline = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Medium,
            UserId = Guid.NewGuid().ToString(),
            Category = Category.Work,
            TaskTags =
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    TagName = "Integration test task"
                }
            ],
            TaskComments =
            [
                new()
                {
                    CommentId = Guid.NewGuid(),
                    Content = "Integration test task"
                }
            ]
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks/addTask", taskDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task DeleteTask_ShouldReturnNoContent_AndPublishEvent()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "Task to be deleted",
            Description = "This task will be deleted",
            Deadline = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Medium,
            UserId = Guid.NewGuid().ToString(),
            Category = Category.Work,
            TaskTags =
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    TagName = "Test tag"
                }
            ],
            TaskComments =
            [
                new()
                {
                    CommentId = Guid.NewGuid(),
                    Content = "Test comment"
                }
            ]
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/tasks/addTask", taskDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var createdTaskId = await createResponse.Content.ReadAsStringAsync();
        
        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/tasks/deleteTask/{createdTaskId}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task UpdateTask_ShouldReturnNoContent_WhenTaskUpdated()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            TaskId = Guid.NewGuid(),
            Title = "Updated Task",
            Description = "Updated Description",
            Category = Category.Work,
            Priority = Priority.Medium,
            Deadline = DateTime.UtcNow.AddDays(1),
            TaskTags = 
            [
                new()
                {
                    TagName = "Updated Tag"
                }
            ]
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/tasks/addTask", taskDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var taskId = await createResponse.Content.ReadAsStringAsync();
        
        var updatedTaskDto = new TaskDto
        {
            TaskId = Guid.Parse(taskId),
            Title = "Updated Task Title",
            Description = "Updated Task Description",
            Category = Category.Work,
            Priority = Priority.Low,
            Deadline = DateTime.UtcNow.AddDays(2)
        };

        // Act
        var updateResponse = await _client.PutAsJsonAsync("/api/tasks/updateTask", updatedTaskDto);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
