using DotNetLearningPlatform.Core;

namespace DotNetLearningPlatform.Application;

public interface ICodeExecutionService
{
    Task<ExecutionResult> ExecuteAsync(string sourceCode, CancellationToken cancellationToken);
}
