using Storage.Domain.Helpers.Abstraction;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Storage.Domain.Helpers.Events {

    public class EventInvoker<TEvent> {
        private readonly ImmutableArray<IEventHandler<TEvent>> _subscriptions;

        public EventInvoker(IEventHandler<TEvent>[] subscriptions) {
            _subscriptions = ImmutableArray.Create(subscriptions);
        }

        public async Task InvokeAsync(TEvent @event) {
            foreach (var handler in _subscriptions)
                await handler.InvokeAsync(@event);
        }
    }
}