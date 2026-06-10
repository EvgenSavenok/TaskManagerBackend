using Application.Contracts.Grpc;
using Application.Contracts.MessagingContracts;
using Application.Contracts.Redis;
using Application.DataTransferObjects.TasksDto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TasksService.Infrastructure;

namespace IntegrationTests;

public class TasksServiceApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Redis mock 
            var cacheDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IRedisCacheService));
            if (cacheDescriptor != null)
                services.Remove(cacheDescriptor);

            var mockCache = new Mock<IRedisCacheService>();
            
            mockCache.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            mockCache.Setup(c => c.GetAsync<object>(It.IsAny<string>()))
                .ReturnsAsync((object?)null);

            mockCache.Setup(c => c.RemoveAsync(It.IsAny<string>()));

            services.AddSingleton(mockCache.Object);
            
            // Database mock
            var dbContextDescriptors = services
                .Where(d => d.ServiceType.FullName != null &&
                            d.ServiceType.FullName.Contains("ApplicationContext"))
                .ToList();

            foreach (var d in dbContextDescriptors)
            {
                services.Remove(d);
            }
            
            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });
            
            // Create task with RabbitMQ
            var taskCreatedDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ITaskCreatedProducer));
            if (taskCreatedDescriptor != null)
                services.Remove(taskCreatedDescriptor);
            
            var taskCreatedPublisher = new Mock<ITaskCreatedProducer>();
            taskCreatedPublisher.Setup(
                p => p.PublishTaskCreatedEvent(It.IsAny<CreateTaskEventDto>()));
            
            services.AddSingleton(taskCreatedPublisher.Object);
            
            // Delete task with RabbitMQ
            var taskDeletedDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ITaskDeletedProducer));
            if (taskDeletedDescriptor != null)
                services.Remove(taskDeletedDescriptor);
            
            var taskDeletedPublisher = new Mock<ITaskDeletedProducer>();
            taskDeletedPublisher.Setup(
                p => p.PublishTaskDeletedEvent(It.IsAny<Guid>()));
            
            services.AddSingleton(taskDeletedPublisher.Object);
            
            // Update task with RabbitMQ
            var taskUpdatedDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ITaskUpdatedProducer));
            if (taskUpdatedDescriptor != null)
                services.Remove(taskUpdatedDescriptor);
            
            var taskUpdatedPublisher = new Mock<ITaskUpdatedProducer>();
            taskUpdatedPublisher.Setup(
                p => p.PublishTaskUpdatedEvent(It.IsAny<UpdateTaskEventDto>()));
            
            services.AddSingleton(taskUpdatedPublisher.Object);
            
            // gRPC
            var grpcClientDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IUserGrpcService));
            if (grpcClientDescriptor != null)
                services.Remove(grpcClientDescriptor);
            
            var mockGrpcClient = new Mock<IUserGrpcService>();
            mockGrpcClient
                .Setup(client => client.GetUserEmailAsync(It.IsAny<string>()))
                .ReturnsAsync("test@example.com");

            services.AddSingleton(mockGrpcClient.Object);
        });
    }
}
