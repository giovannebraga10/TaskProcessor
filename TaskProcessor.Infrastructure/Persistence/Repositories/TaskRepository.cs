using MongoDB.Driver;
using TaskProcessor.Domain.Aggregates.TaskAggregate;
using TaskProcessor.Domain.Aggregates.TaskAggregate.Interfaces;
using TaskProcessor.Infrastructure.Persistence.Interfaces;
namespace TaskProcessor.Infrastructure.Persistence.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IMongoCollection<AppTask> _tasks;

        public TaskRepository(IMongoDbContext context)
        {
            _tasks = context.Tasks;
        }
        public async Task AddAsync(AppTask task) =>
            await _tasks.InsertOneAsync(task);

        public async Task<AppTask> GetByIdAsync(Guid id) =>
            await _tasks.Find(t => t.Id == id).FirstOrDefaultAsync();

        public async Task UpdateAsync(AppTask task) =>
            await _tasks.ReplaceOneAsync(t => t.Id == task.Id, task);
    }
}
