using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _config;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient(IConfiguration config)
    {
        _config = config;

        var factory = new ConnectionFactory(){
            HostName = _config["RabbitMQHost"],
            Port = int.Parse(_config["RabbitMQPort"])
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "Trigger",type: ExchangeType.Fanout);
            _connection.ConnectionShutdown += RabbitMQConnectionShutdown;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"---> Couldn't connect to RabbitMQ message bus : {ex.Message}");
        }
   
        Console.WriteLine("---> Connected to MessageBus");
    }


    public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
        var message = JsonSerializer.Serialize(platformPublishedDto);
        if (_connection.IsOpen)
        {
            Console.WriteLine($"---> RabbitMQ open : sending messages");
            SendMessage(message);
        }else
        {
            Console.WriteLine("---> RabbitMQ Connection losed, not sending");
        }
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(
            exchange: "Trigger",
            routingKey: "",
            basicProperties: null,
            body: body);
        Console.WriteLine($"Message Sent : {message}");
    }

    private void Dispose(){

        Console.WriteLine("---> Disposed Of MessageBus");
        if (_connection.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }
    }

    private void RabbitMQConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine("---> RabbitMQ Connection Shutdown");
    }
}