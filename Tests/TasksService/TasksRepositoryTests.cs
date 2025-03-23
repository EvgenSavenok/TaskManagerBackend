using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TasksService.Domain.Enums;
using TasksService.Domain.Models;
using TasksService.Infrastructure;
using TasksService.Infrastructure.Repositories;

namespace Tests.TasksService;

public class TasksRepositoryTests : IDisposable
{
    private readonly ApplicationContext _context;
    private readonly TasksRepository _repository;

    public TasksRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;

        _context = new ApplicationContext(options);
        _repository = new TasksRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnTask_WhenTaskExists()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        
        var task = new CustomTask
        {
            Id = taskId,
            UserId = Guid.NewGuid(),
            Title = "Title",
            Description = "Description",
            Category = Category.Work,
            Priority = Priority.High,
            Deadline = DateTime.UtcNow.AddDays(5),
        };

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTaskByIdAsync(
            taskId, 
            trackChanges: false,
            CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(taskId);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnNull_WhenTaskDoesNotExist()
    {
        // Act
        var result = await _repository.GetTaskByIdAsync(
            Guid.NewGuid(),
            trackChanges: false, 
            CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task FindAll_ReturnsAllTasks()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        await _context.Tasks.AddRangeAsync(
            new CustomTask
            {
                Id = Guid.NewGuid(), 
                UserId = userId,
                Title = "Title",
                Description = "Description",
                Category = Category.Work,
                Priority = Priority.High,
                Deadline = DateTime.UtcNow.AddDays(5),
            },
            new CustomTask
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = "Title",
                Description = "Description",
                Category = Category.Work,
                Priority = Priority.High,
                Deadline = DateTime.UtcNow.AddDays(5),
            }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllTasks(
            trackChanges: false, 
            CancellationToken.None,
            userId);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Create_AddsTaskToDatabase()
    {
        // Arrange
        var deadline = DateTime.UtcNow.AddDays(10);
        
        var task = new CustomTask
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Title = "Title",
            Description = "Description",
            Category = Category.Work,
            Priority = Priority.High,
            Deadline = deadline,
            TaskTags = new List<Tag>
            {
                new() { Id = Guid.NewGuid(), Name = "Urgent" },
                new() { Id = Guid.NewGuid(), Name = "Work" }
            },
            TaskComments = new List<Comment>
            {
                new() { Id = Guid.NewGuid(), Content = "First comment" },
                new() { Id = Guid.NewGuid(), Content = "Second comment" }
            }
        };

        // Act
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
        
        var result = await _context.Tasks.FindAsync(task.Id);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Title");
        result.Description.Should().Be("Description");
        result.Category.Should().Be(Category.Work);
        result.Priority.Should().Be(Priority.High);
        result.Deadline.Should().Be(deadline);
        
        result.TaskTags.Should().NotBeNull();
        result.TaskTags.Should().HaveCount(2);
        result.TaskTags.Select(t => t.Name).Should().Contain(new[] { "Urgent", "Work" });

        result.TaskComments.Should().NotBeNull();
        result.TaskComments.Should().HaveCount(2);
        result.TaskComments.Select(c => c.Content).Should().Contain(
            new[] { "First comment", "Second comment" });
    }

    [Fact]
    public async Task Update_UpdatesTaskInDatabase()
    {
        // Arrange
        var deadline = DateTime.UtcNow.AddDays(10);
        
        var task = new CustomTask 
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Title = "Title",
            Description = "Description",
            Category = Category.Work,
            Priority = Priority.High,
            Deadline = deadline,
            TaskTags = new List<Tag>
            {
                new() { Id = Guid.NewGuid(), Name = "Urgent" },
                new() { Id = Guid.NewGuid(), Name = "Work" }
            },
            TaskComments = new List<Comment>
            {
                new() { Id = Guid.NewGuid(), Content = "First comment" },
                new() { Id = Guid.NewGuid(), Content = "Second comment" }
            }
        };

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        var existingTask = await _context.Tasks
            .Include(t => t.TaskTags) 
            .Include(t => t.TaskComments) 
            .FirstOrDefaultAsync(t => t.Id == task.Id);

        existingTask.Should().NotBeNull();

        // Act
        existingTask.Title = "Updated Title";

        var urgentTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == "Urgent");
        urgentTag.Should().NotBeNull();

        existingTask.TaskTags = new List<Tag> { urgentTag };

        await _repository.Update(existingTask, CancellationToken.None);

        var result = await _context.Tasks
            .Include(t => t.TaskTags)
            .Include(t => t.TaskComments)
            .FirstOrDefaultAsync(t => t.Id == task.Id);

        // Assert
        result.Title.Should().Be("Updated Title");
        result.TaskTags.Should().NotBeNull();
        result.TaskTags.Should().HaveCount(1);
        result.TaskTags.Select(t => t.Name).Should().Contain(new[] { "Urgent" });
    }

    [Fact]
    public async Task Delete_RemovesTaskAndRelatedEntitiesFromDatabase()
    {
        // Arrange
        var deadline = DateTime.UtcNow.AddDays(10);
    
        var task = new CustomTask 
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Title = "Title",
            Description = "Description",
            Category = Category.Work,
            Priority = Priority.High,
            Deadline = deadline,
            TaskTags = new List<Tag>
            {
                new() { Id = Guid.NewGuid(), Name = "Urgent" },
                new() { Id = Guid.NewGuid(), Name = "Work" }
            },
            TaskComments = new List<Comment>
            {
                new() { Id = Guid.NewGuid(), Content = "First comment" },
                new() { Id = Guid.NewGuid(), Content = "Second comment" }
            }
        };
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        var existingTask = await _context.Tasks
            .Include(t => t.TaskTags)
            .Include(t => t.TaskComments)
            .FirstOrDefaultAsync(t => t.Id == task.Id);

        existingTask.Should().NotBeNull();
        existingTask.TaskTags.Should().NotBeEmpty();
        existingTask.TaskComments.Should().NotBeEmpty();

        // Act
        await _repository.Delete(existingTask, CancellationToken.None);

        var deletedTask = await _context.Tasks.FindAsync(task.Id);
        deletedTask.Should().BeNull();

        var deletedComments = await _context.Comments
            .Where(c => c.TaskId == task.Id)
            .ToListAsync();
        deletedComments.Should().BeEmpty();

        var deletedTaskTags = await _context.Tags
            .Where(t => t.TaskTags.Any(task => task.Id == task.Id))
            .ToListAsync();
        deletedTaskTags.Should().BeEmpty();
    }
}
