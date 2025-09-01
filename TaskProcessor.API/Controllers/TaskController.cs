using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskProcessor.Application.Commands.CreateTask;
using TaskProcessor.Application.Queries.GetTaskStatus;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { Id = id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStatus(Guid id)
    {
        var status = await _mediator.Send(new GetTaskStatusQuery(id));
        return Ok(new { Status = status.ToString() });
    }
}
