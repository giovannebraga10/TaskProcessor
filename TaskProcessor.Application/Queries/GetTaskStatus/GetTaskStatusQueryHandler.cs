using MediatR;
using TaskProcessor.Domain.Aggregates.TaskAggregate;
using TaskProcessor.Domain.Aggregates.TaskAggregate.Interfaces;

namespace TaskProcessor.Application.Queries.GetTaskStatus
{
    public class GetTaskStatusQueryHandler(ITaskRepository _taskRepository) : IRequestHandler<GetTaskStatusQuery, ETaskStatus>
    {
        public async Task<ETaskStatus> Handle(GetTaskStatusQuery request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.id);
            if (task == null) throw new KeyNotFoundException("Task not found.");

            return task.Status;
        }
    }
}
