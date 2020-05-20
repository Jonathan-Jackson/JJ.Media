using System;
using System.Threading;
using System.Threading.Tasks;

namespace JJ.HostedService.Abstraction {

    public interface IBackgroundTaskQueue {

        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);

        void Enqueue(Func<CancellationToken, Task> workItem);
    }
}