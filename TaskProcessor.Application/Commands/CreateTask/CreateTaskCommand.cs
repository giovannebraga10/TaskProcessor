using MediatR;
using TaskProcessor.Domain.Aggregates.TaskAggregate;

namespace TaskProcessor.Application.Commands.CreateTask
{
    public record CreateTaskCommand(string payload, ETaskType typeTask) : IRequest<Guid>;
}
