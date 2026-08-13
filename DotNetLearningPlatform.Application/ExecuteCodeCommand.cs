using DotNetLearningPlatform.Core;
using MediatR;

namespace DotNetLearningPlatform.Application;

public class ExecuteCodeCommand : IRequest<ExecutionResult>
{
    public string SourceCode { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
}
