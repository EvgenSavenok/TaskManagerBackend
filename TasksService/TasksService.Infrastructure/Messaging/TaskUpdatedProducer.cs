using System.Text;
using System.Text.Json;
using Application.Contracts.MessagingContracts;
using Application.DataTransferObjects.TasksDto;
using RabbitMQ.Client;

namespace TasksService.Infrastructure.Messaging;

public class TaskUpdatedProducer : ITaskUpdatedProducer
{
    private readonly IModel _channel;
    private const string ExchangeName = "update_task_exchange";
    
    public TaskUpdatedProducer()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);
    }
    
    public void PublishTaskUpdatedEvent(UpdateTaskEventDto createTaskEventDto)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(createTaskEventDto));
        _channel.BasicPublish(exchange: ExchangeName, routingKey: "", body: body);
    }
}