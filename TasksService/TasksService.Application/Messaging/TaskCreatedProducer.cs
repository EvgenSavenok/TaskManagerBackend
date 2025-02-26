using System.Text;
using System.Text.Json;
using Application.DataTransferObjects.TasksDto;
using RabbitMQ.Client;

namespace Application.Messaging;

public class TaskCreatedProducer : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string ExchangeName = "create_task_exchange";

    public TaskCreatedProducer()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);
    }
    
    public void PublishTaskCreatedEvent(TaskEventDto taskEventDto)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(taskEventDto));
        _channel.BasicPublish(exchange: ExchangeName, routingKey: "", body: body);
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}
