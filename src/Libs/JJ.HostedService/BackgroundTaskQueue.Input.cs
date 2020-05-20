﻿using JJ.HostedService.Abstraction;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace JJ.HostedService {

    public class BackgroundTaskQueue<TInput> : IBackgroundTaskQueue<TInput> {
        private readonly ConcurrentQueue<Func<TInput, CancellationToken, Task>> _workItems;
        private readonly SemaphoreSlim _semaphore;

        public BackgroundTaskQueue() {
            _workItems = new ConcurrentQueue<Func<TInput, CancellationToken, Task>>();
            _semaphore = new SemaphoreSlim(0);
        }

        public void Enqueue(Func<TInput, CancellationToken, Task> workItem) {
            if (workItem == null)
                throw new ArgumentNullException(nameof(workItem));

            _workItems.Enqueue(workItem);
            _semaphore.Release();
        }

        public async Task<Func<TInput, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken) {
            await _semaphore.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }
}