using DotNetLearningPlatform.Application;
using DotNetLearningPlatform.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddPolicy("LocalBlazor", policy =>
    policy.WithOrigins("http://localhost:5173", "https://localhost:7173")
        .AllowAnyHeader()
        .AllowAnyMethod()));

// Configure MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ExecuteCodeCommand).Assembly));

// Configure Infrastructure
builder.Services.AddScoped<ICodeExecutionService, RoslynCodeExecutionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("LocalBlazor");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
