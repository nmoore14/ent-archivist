using Archivist.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddPolicy("development-web", policy => policy
        .WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseCors("development-web");
}
app.MapOpenApi();
app.MapHealthChecks("/health").AllowAnonymous();
app.MapGet("/api/v1/status", () => Results.Ok(new { service = "archivist-api", status = "ready" }))
    .AllowAnonymous();
app.MapKnowledgeContracts();

app.Run();

public partial class Program;
