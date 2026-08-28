using DotNetLearningPlatform.Application;
using DotNetLearningPlatform.Core;
using DotNetLearningPlatform.Infrastructure;
using Xunit;

namespace DotNetLearningPlatform.Tests;

public class CodeExecutionTests
{
    [Fact]
    public async Task Executes_csharp_that_writes_output()
    {
        var result = await new RoslynCodeExecutionService().ExecuteAsync(
            "using System; Console.WriteLine(\"hello\");", CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Contains("hello", result.Output);
    }

    [Fact]
    public async Task Returns_compilation_error_for_invalid_csharp()
    {
        var result = await new RoslynCodeExecutionService().ExecuteAsync(
            "using System; Console.WriteLine(;", CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Compilation failed", result.Error);
    }

    [Fact]
    public async Task Rejects_empty_source_and_unsupported_language()
    {
        var handler = new ExecuteCodeCommandHandler(new ThrowingExecutionService());

        var empty = await handler.Handle(new ExecuteCodeCommand
        {
            SourceCode = " ",
            Language = "csharp"
        }, CancellationToken.None);
        var language = await handler.Handle(new ExecuteCodeCommand
        {
            SourceCode = "Console.WriteLine(1);",
            Language = "python"
        }, CancellationToken.None);

        Assert.Equal("Source code cannot be empty.", empty.Error);
        Assert.Contains("Only C#", language.Error);
    }

    [Fact]
    public async Task Returns_runtime_error()
    {
        var result = await new RoslynCodeExecutionService().ExecuteAsync(
            "using System; throw new Exception(\"boom\");", CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("boom", result.Error);
    }

    [Fact]
    public async Task Returns_timeout_for_non_terminating_code()
    {
        var result = await new RoslynCodeExecutionService().ExecuteAsync(
            "while (true) { }", CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("timed out", result.Error);
    }

    [Fact]
    public async Task Returns_cancellation_message_when_request_is_cancelled()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        var result = await new RoslynCodeExecutionService().ExecuteAsync(
            "Console.WriteLine(1);", cancellation.Token);

        Assert.False(result.IsSuccess);
        Assert.Contains("cancellation was requested", result.Error);
    }

    private sealed class ThrowingExecutionService : ICodeExecutionService
    {
        public Task<ExecutionResult> ExecuteAsync(string sourceCode, CancellationToken cancellationToken) =>
            throw new InvalidOperationException("Should not execute invalid requests.");
    }
}
