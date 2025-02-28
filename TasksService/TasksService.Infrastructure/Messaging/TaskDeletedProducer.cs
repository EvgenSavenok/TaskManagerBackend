using System.Text;
using System.Text.Json;
using Application.Contracts.MessagingContracts;
using RabbitMQ.Client;

namespace TasksService.Infrastructure.Messaging;

public class TaskDeletedProducer : ITaskDeletedProducer
{
    
    private readonly IModel _channel;
    private const string ExchangeName = "delete_task_exchange";
    
    public TaskDeletedProducer()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);
    }
    
    public void PublishTaskDeletedEvent(Guid taskId)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(taskId));
        _channel.BasicPublish(exchange: ExchangeName, routingKey: "", body: body);
    }
}