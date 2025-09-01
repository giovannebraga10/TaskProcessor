using TaskProcessor.Domain.Aggregates.TaskAggregate;

namespace TaskProcessor.Infrastructure.Messaging
{
    public interface ITaskPublisher
    {
        Task PublishTaskAsync(AppTask task);
    }
}
