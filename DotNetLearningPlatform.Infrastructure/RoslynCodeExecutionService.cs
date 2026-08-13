using DotNetLearningPlatform.Application;
using DotNetLearningPlatform.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace DotNetLearningPlatform.Infrastructure;

public class RoslynCodeExecutionService : ICodeExecutionService
{
    public async Task<ExecutionResult> ExecuteAsync(string sourceCode, CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            try
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode, cancellationToken: cancellationToken);

                var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location) ?? string.Empty;
                var references = new MetadataReference[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                    MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll"))
                };

                var compilation = CSharpCompilation.Create(
                    "DynamicCodeExecution",
                    syntaxTrees: new[] { syntaxTree },
                    references: references,
                    options: new CSharpCompilationOptions(OutputKind.ConsoleApplication));

                using var ms = new MemoryStream();
                var emitResult = compilation.Emit(ms, cancellationToken: cancellationToken);

                if (!emitResult.Success)
                {
                    var errors = string.Join(Environment.NewLine, emitResult.Diagnostics
                        .Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error)
                        .Select(d => d.GetMessage()));
                    return ExecutionResult.Failure($"Compilation failed:{Environment.NewLine}{errors}");
                }

                ms.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(ms.ToArray());
                var entryPoint = assembly.EntryPoint;

                if (entryPoint == null)
                {
                    return ExecutionResult.Failure("No entry point found.");
                }

                var originalOut = Console.Out;
                using var stringWriter = new StringWriter();
                Console.SetOut(stringWriter);

                try
                {
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    cts.CancelAfter(TimeSpan.FromSeconds(3));

                    var task = Task.Run(() =>
                    {
                        var parameters = entryPoint.GetParameters().Length == 0 ? null : new object[] { Array.Empty<string>() };
                        entryPoint.Invoke(null, parameters);
                    }, cts.Token);

                    task.Wait(cts.Token);
                }
                finally
                {
                    Console.SetOut(originalOut);
                }

                return ExecutionResult.Success(stringWriter.ToString());
            }
            catch (OperationCanceledException)
            {
                return ExecutionResult.Failure("Execution timed out (3 seconds limit).");
            }
            catch (Exception ex)
            {
                return ExecutionResult.Failure($"Execution failed: {ex.InnerException?.Message ?? ex.Message}");
            }
        }, cancellationToken);
    }
}
