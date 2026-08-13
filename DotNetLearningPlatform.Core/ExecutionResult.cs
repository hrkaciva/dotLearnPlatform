namespace DotNetLearningPlatform.Core;

public class ExecutionResult
{
    public bool IsSuccess { get; set; }
    public string Output { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;

    public static ExecutionResult Success(string output) => new() { IsSuccess = true, Output = output };
    public static ExecutionResult Failure(string error) => new() { IsSuccess = false, Error = error };
}
