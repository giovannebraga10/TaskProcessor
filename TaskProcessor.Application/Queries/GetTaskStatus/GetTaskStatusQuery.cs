using MediatR;
using TaskProcessor.Domain.Aggregates.TaskAggregate;

namespace TaskProcessor.Application.Queries.GetTaskStatus
{
    public record GetTaskStatusQuery(Guid id) : IRequest<ETaskStatus>;
}
