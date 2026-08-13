using DotNetLearningPlatform.Core;
using MediatR;

namespace DotNetLearningPlatform.Application;

public class ExecuteCodeCommandHandler : IRequestHandler<ExecuteCodeCommand, ExecutionResult>
{
    private readonly ICodeExecutionService _executionService;

    public ExecuteCodeCommandHandler(ICodeExecutionService executionService)
    {
        _executionService = executionService;
    }

    public async Task<ExecutionResult> Handle(ExecuteCodeCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SourceCode))
        {
            return ExecutionResult.Failure("Source code cannot be empty.");
        }

        return await _executionService.ExecuteAsync(request.SourceCode, cancellationToken);
    }
}
