using MongoDB.Driver;
using TaskProcessor.Domain.Aggregates.TaskAggregate;

namespace TaskProcessor.Infrastructure.Persistence.Interfaces
{
    public interface IMongoDbContext
    {
        IMongoCollection<AppTask> Tasks { get; }
    }
}
