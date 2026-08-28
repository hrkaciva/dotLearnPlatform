using DotNetLearningPlatform.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotNetLearningPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CodeController : ControllerBase
{
    private readonly IMediator _mediator;

    public CodeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("execute")]
    public async Task<IActionResult> ExecuteCode(
        [FromBody] ExecuteCodeCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}
