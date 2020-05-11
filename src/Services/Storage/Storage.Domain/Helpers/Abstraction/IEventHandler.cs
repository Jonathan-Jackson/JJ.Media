using System.Threading.Tasks;

namespace Storage.Domain.Helpers.Abstraction {

    public interface IEventHandler<TEvent> {

        public Task InvokeAsync(TEvent @event);
    }
}