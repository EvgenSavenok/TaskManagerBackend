﻿using System.Text;
using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationsService.Application.DataTransferObjects.NotificationsDto;
using NotificationsService.Application.DataTransferObjects.TaskEventDto;
using NotificationsService.Application.UseCases.Commands.NotificationCommands.CreateNotification;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace NotificationsService.Infrastructure.Messaging;

public class TaskCreatedConsumer : BackgroundService
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly IServiceProvider _serviceProvider;
    private const string ExchangeName = "create_task_exchange";
    private const string QueueName = "created_task_notifications_queue";

    public TaskCreatedConsumer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq_taskmanager"
        };
        
        for (int i = 0; i < 5; i++)
        {
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
            }
            catch (Exception)
            {
                Console.WriteLine($"[RabbitMQ] Не удалось подключиться, попытка {i + 1}...");
                Thread.Sleep(10000); 
            }
        }

        Console.WriteLine("[RabbitMQ] Соединение с RabbitMQ для TaskCreatedConsumer установлено.");
        _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);
        _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: "");
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            try
            {
                var taskCreatedEvent = JsonSerializer.Deserialize<CreateTaskEventDto>(message);

                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                var notificationDto = mapper.Map<NotificationDto>(taskCreatedEvent);
            
                var command = new CreateNotificationCommand
                {
                    NotificationDto = notificationDto
                };
                await mediator.Send(command, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RabbitMQ Error] {ex.Message}"); 
            }
        };

        _channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
    
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel.Close();
        _connection.Close(); 
        return base.StopAsync(cancellationToken);
    }
}