using System;
using System.Threading;
using System.Threading.Tasks;

namespace JJ.Framework.Repository.Abstraction {

    public interface IMessageBroker {

        void DeclareQueue(string queueName);

        void BindQueue(string exchangeName, string queueName, string routingKey);

        void Publish<TObject>(string routingKey, TObject model, string exchangeName = "");

        void DeclareExchange(string exchangeName, string type = "direct");

        Task RecieverAsync<TObject>(string queueName, Action<TObject> onRecieve, CancellationToken? stoppingToken = null);

        Task RecieverAsync<TObject>(string queueName, Func<TObject, Task> onRecieve, CancellationToken? stoppingToken = null);

        Task RecieverAsync(string queueName, Func<string, Task> onRecieve, CancellationToken? stoppingToken = null);

        bool TryConnect();
    }
}