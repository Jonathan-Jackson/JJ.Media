﻿using JJ.Framework.Repository.Abstraction;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace JJ.Framework.Repository.RabbitMq {

    public class RabbitBroker : IMessageBroker {
        private readonly ConnectionFactory _connFactory;
        private readonly ILogger<RabbitBroker> _logger;
        private readonly object _semaphore = new object();
        private IConnection _conn;
        private IModel _channel;

        public RabbitBroker(string hostName, string userName, string password, ILogger<RabbitBroker> logger = null) {
            _connFactory = new ConnectionFactory {
                HostName = hostName,
                UserName = userName,
                Password = password
            };
            _logger = logger;
        }

        public void BindQueue(string exchangeName, string queueName, string routingKey) {
            ProcessChannel((channel) => {
                channel.QueueBind(exchange: exchangeName,
                                    queue: queueName,
                                    routingKey: routingKey);

                _logger?.LogDebug($"BOUND QUEUE: {queueName} ON EXCHANGE: {exchangeName} WITH KEY: {routingKey}");
            });
        }

        public void DeclareExchange(string exchangeName, string type = "direct") {
            ProcessChannel((channel) => {
                channel.ExchangeDeclare(exchange: exchangeName, type: "direct");
                _logger?.LogDebug($"DECLARED EXCHANGE: {exchangeName}");
            });
        }

        public void DeclareQueue(string queueName) {
            ProcessChannel((channel) => {
                channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            });
        }

        public void Publish<TObject>(string routingKey, TObject model, string exchangeName = "") {
            ProcessChannel((channel) => {
                string message = JsonSerializer.Serialize(model);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: exchangeName,
                                     routingKey: routingKey,
                                     basicProperties: null,
                                     body: body);

                _logger?.LogDebug($"PUBLISHED MESSAGE: {message} WITH KEY: {routingKey} ON EXCHANGE: {exchangeName}");
            });
        }

        public Task RecieverAsync<TObject>(string queueName, Action<TObject> onRecieve, CancellationToken? stoppingToken = null) {
            return ProcessChannelAsync(async (channel) => {
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (_, ea) => {
                    TObject model = JsonSerializer.Deserialize<TObject>(ea.Body.Span);
                    onRecieve(model);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                _logger?.LogDebug($"AWAITING QUEUE: {queueName}");

                if (stoppingToken.HasValue)
                    await Task.Delay(-1, stoppingToken.Value);
                else
                    await Task.Delay(-1);
            });
        }

        public Task RecieverAsync(string queueName, Func<string, Task> onRecieve, CancellationToken? stoppingToken = null) {
            return ProcessChannelAsync(async (channel) => {
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (_, ea) => {
                    var text = Encoding.UTF8.GetString(ea.Body.ToArray());
                    _logger.LogDebug($"Broker Message Recieved On Queue {queueName}: {text}");
                    await onRecieve(text);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                _logger?.LogDebug($"AWAITING QUEUE: {queueName}");

                if (stoppingToken.HasValue)
                    await Task.Delay(-1, stoppingToken.Value);
                else
                    await Task.Delay(-1);

                _logger.LogWarning($"Closing Reciever on queue: {queueName}");
            });
        }

        public Task RecieverAsync<TObject>(string queueName, Func<TObject, Task> onRecieve, CancellationToken? stoppingToken = null) {
            return ProcessChannelAsync(async (channel) => {
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (_, ea) => {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    TObject model = JsonSerializer.Deserialize<TObject>(json);
                    _logger.LogDebug($"Broker Message Recieved On Queue {queueName}: {json}");
                    await onRecieve(model);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                _logger?.LogDebug($"AWAITING QUEUE: {queueName}");

                if (stoppingToken.HasValue)
                    await Task.Delay(-1, stoppingToken.Value);
                else
                    await Task.Delay(-1);

                _logger.LogWarning($"Closing Reciever on queue: {queueName}");
            });
        }

        public bool TryConnect() {
            try {
                GetChannel();
                return true;
            }
            catch {
                return false;
            }
        }

        private IModel GetChannel() {
            if (_channel != null && _channel.IsOpen)
                return _channel;

            lock (_semaphore) {
                if (_conn?.IsOpen != true) {
                    _conn?.Dispose();
                    _conn = _connFactory.CreateConnection();
                }

                if (_channel?.IsOpen != true) {
                    _channel?.Dispose();
                    _channel = _conn.CreateModel();
                }
            }

            return _channel;
        }

        private void ProcessChannel(Action<IModel> action) {
            using (var channel = GetChannel())
                action(channel);
        }

        private async Task ProcessChannelAsync(Func<IModel, Task> action) {
            using (var channel = GetChannel())
                await action(channel);
        }
    }
}