using MediatR;
using TaskProcessor.Domain.Aggregates.TaskAggregate;
using TaskProcessor.Domain.Aggregates.TaskAggregate.Interfaces;
using TaskProcessor.Infrastructure.Messaging;

namespace TaskProcessor.Application.Commands.CreateTask
{
    public class CreateTaskCommandHandler(
        ITaskRepository _taskRepository,
        ITaskPublisher _taskPublisher) : IRequestHandler<CreateTaskCommand, Guid>
    {
        public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = AppTask.Create(request.typeTask, request.payload);
            await _taskRepository.AddAsync(task);
            await _taskPublisher.PublishTaskAsync(task);

            return task.Id;
        }

    }
}
