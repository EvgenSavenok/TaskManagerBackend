﻿using System.Text;
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
    private readonly IConnection _connection;
    private readonly IServiceProvider _serviceProvider;
    private const string ExchangeName = "update_task_exchange";
    private const string QueueName = "updated_task_notifications_queue";

    public TaskUpdatedConsumer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq_taskmanager",
            UserName = "guest",
            Password = "guest"
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

        Console.WriteLine("[RabbitMQ] Соединение с RabbitMQ для TaskUpdatedConsumer установлено.");
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
            
            try 
            {
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