using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TaskProcessor.Domain.Aggregates.TaskAggregate;

namespace TaskProcessor.Infrastructure.Messaging
{
    public class TaskPublisher : ITaskPublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMqSettings _settings;
        private bool _disposed;

        public TaskPublisher(IOptions<RabbitMqSettings> options)
        {
            _settings = options.Value;

            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password,
                Port = _settings.Port
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            var dlqName = $"{_settings.QueueName}_dlq";
            _channel.QueueDeclare(dlqName, true, false, false, null);

            var args = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "" },
                { "x-dead-letter-routing-key", dlqName }
            };

            _channel.QueueDeclare(_settings.QueueName, true, false, false, args);
        }

        public Task PublishTaskAsync(AppTask task)
        {
            var message = Newtonsoft.Json.JsonConvert.SerializeObject(task);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish("", _settings.QueueName, properties, body);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _channel?.Close();
                _connection?.Close();
                _disposed = true;
            }
        }
    }
}
