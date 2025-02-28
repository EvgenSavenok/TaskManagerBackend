using System.Text;
using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationsService.Application.DataTransferObjects.NotificationsDto;
using NotificationsService.Application.DataTransferObjects.TaskEventDto;
using NotificationsService.Application.UseCases.Commands.NotificationCommands.UpdateNotification;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationsService.Infrastructure.Messaging;

public class TaskUpdatedConsumer : BackgroundService
{
    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;
    private const string ExchangeName = "update_task_exchange";
    private const string QueueName = "updated_task_notifications_queue";

    public TaskUpdatedConsumer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();

        _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);
        _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: "");
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var taskUpdatedEvent = JsonSerializer.Deserialize<UpdateTaskEventDto>(message);

            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

            var notificationDto = mapper.Map<NotificationDto>(taskUpdatedEvent);
        
            var command = new UpdateNotificationCommand
            {
                NotificationDto = notificationDto
            };

            await mediator.Send(command, stoppingToken);

            _channel.BasicAck(eventArgs.DeliveryTag, false);
        };

        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }
}