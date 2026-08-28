# .NET Learning Platform

This is a personal learning project for building a .NET application while learning the concepts behind it. The long-term goal is to create tutorial-based learning labs for .NET, inspired by platforms such as Scrimba, where lessons combine explanations, guided exercises, and executable code. This editor-to-API execution flow is only the first step in that implementation. For now, you can write C# in the browser editor, run it through the ASP.NET Core API, and inspect the real program output or errors in the terminal panel. The project is intentionally split into a Blazor client, API, application layer, and infrastructure layer so you can learn how these parts work together while experimenting with C# and .NET concepts.

## Local development

Run the API in one terminal:

```text
dotnet run --project DotNetLearningPlatform.Api --launch-profile http
```

The API is available at `http://localhost:5041`. Run the Blazor host in another terminal:

```text
dotnet run --project DotNetLearningPlatform --launch-profile http
```

The Blazor editor is available at `http://localhost:5173`. The client is configured to call the API at `http://localhost:5041`; the API permits only the local Blazor origins `http://localhost:5173` and `https://localhost:7173` in Development. The API HTTPS profile is `https://localhost:7155`.

## Code execution API

`POST /api/code/execute` accepts JSON containing `sourceCode` and `language`. The only supported language is C# and must be sent as `"csharp"`:

```json
{
  "sourceCode": "Console.WriteLine(\"Hello from the API\");",
  "language": "csharp"
}
```

Successful and failed executions return an `ExecutionResult` with `isSuccess`, `output`, and `error` fields.

## Security limitation

The current Roslyn executor is not a secure sandbox. Submitted code executes in the API process and must not be exposed to untrusted users. Its timeout and request cancellation handling do not safely terminate arbitrary code that is already running.
