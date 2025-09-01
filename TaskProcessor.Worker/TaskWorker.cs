using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using TaskProcessor.Application.Services;
using TaskProcessor.Domain.Aggregates.TaskAggregate;
using TaskProcessor.Domain.Aggregates.TaskAggregate.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using TaskProcessor.Infrastructure.Persistence.Repositories;
using MongoDB.Bson;

namespace TaskProcessor.Infrastructure.Messaging
{
    public class TaskWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RabbitMqSettings _settings;
        private IConnection _connection;
        private IModel _channel;
        private const int MaxRetries = 3;
        private bool _disposed;

        public TaskWorker(IServiceScopeFactory scopeFactory, RabbitMqSettings settings)
        {
            _scopeFactory = scopeFactory;
            _settings = settings;

            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password,
                Port = _settings.Port
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            var args = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "" },
                { "x-dead-letter-routing-key", $"{_settings.QueueName}_dlq" }
            };

            _channel.QueueDeclare(_settings.QueueName, true, false, false, args);
            _channel.QueueDeclare($"{_settings.QueueName}_dlq", true, false, false, null);
            _channel.BasicQos(0, 5, false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var jsonString = Encoding.UTF8.GetString(body);

                var task = Newtonsoft.Json.JsonConvert.DeserializeObject<AppTask>(jsonString);

                int retryCount = 0;
                if (ea.BasicProperties.Headers != null &&
                    ea.BasicProperties.Headers.TryGetValue("x-retry", out var r))
                    retryCount = Convert.ToInt32(r);

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
                    var taskProcessorService = scope.ServiceProvider.GetRequiredService<ITaskProcessorService>();

                    switch (task.Type)
                    {
                        case ETaskType.SendEmail:
                            await taskProcessorService.ProcessSendEmail(task.Payload);
                            break;
                        case ETaskType.GenerateReport:
                            await taskProcessorService.ProcessGenerateReport(task.Payload);
                            break;
                    }

                    task.MarkAsCompleted();
                    await taskRepository.UpdateAsync(task);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch
                {
                    retryCount++;

                    if (retryCount < MaxRetries)
                    {
                        var props = _channel.CreateBasicProperties();
                        props.Persistent = true;
                        props.Headers = new Dictionary<string, object>
                        {
                            { "x-retry", retryCount }
                        };

                        _channel.BasicPublish("", _settings.QueueName, props, body);
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                    else
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
                        task.Cancel();
                        await taskRepository.UpdateAsync(task);
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                }
            };

            _channel.BasicConsume(_settings.QueueName, false, consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                _channel?.Close();
                _connection?.Close();
                _disposed = true;
            }
            base.Dispose();
        }
    }
}
