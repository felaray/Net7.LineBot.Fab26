using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

var logger = new LoggerConfiguration()
  .WriteTo.ApplicationInsights(
    builder.Services.BuildServiceProvider().GetService<TelemetryConfiguration>(),
    TelemetryConverter.Traces)
  .CreateLogger();


app.MapPost("/message", async (HttpContext context) =>
{

    try
    {
        var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
        await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
        var requestContent = Encoding.UTF8.GetString(buffer);
        context.Request.EnableBuffering();
        context.Request.Body.Position = 0;
        string postData = requestContent;

        logger.Information("Hello, Serilog!");
        return Results.Ok();
    }catch(Exception ex)
    {
        logger.Error("Hello, Serilog!");
        return Results.Ok();
    }

})
.WithName("GetLineMessage");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}