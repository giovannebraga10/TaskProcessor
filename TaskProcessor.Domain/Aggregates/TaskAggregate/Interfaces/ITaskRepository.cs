namespace TaskProcessor.Domain.Aggregates.TaskAggregate.Interfaces
{
    public interface ITaskRepository
    {
        Task AddAsync(AppTask task);                      
        Task<AppTask> GetByIdAsync(Guid id);
        Task UpdateAsync(AppTask task);                   
    }
}
