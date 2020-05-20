using System;
using System.Threading;
using System.Threading.Tasks;

namespace JJ.HostedService.Abstraction {

    public interface IBackgroundTaskQueue<TInput> {

        Task<Func<TInput, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);

        void Enqueue(Func<TInput, CancellationToken, Task> workItem);
    }
}