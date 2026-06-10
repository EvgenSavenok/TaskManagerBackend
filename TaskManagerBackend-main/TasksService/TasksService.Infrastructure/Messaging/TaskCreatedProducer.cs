using System.Text;
using System.Text.Json;
using Application.Contracts.MessagingContracts;
using Application.DataTransferObjects.TasksDto;
using RabbitMQ.Client;

namespace TasksService.Infrastructure.Messaging;

public class TaskCreatedProducer : ITaskCreatedProducer
{
    private readonly IModel _channel;
    private const string ExchangeName = "create_task_exchange";

    public TaskCreatedProducer()
    {
        var factory = new ConnectionFactory { HostName = "rabbitmq_taskmanager" };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);
    }
    
    public void PublishTaskCreatedEvent(CreateTaskEventDto createTaskEventDto)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(createTaskEventDto));
        _channel.BasicPublish(exchange: ExchangeName, routingKey: "", body: body);
    }
}
