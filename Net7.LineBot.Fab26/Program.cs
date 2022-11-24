using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapPost("/message", async (HttpContext context) =>
{
    var log = new LoggerConfiguration()
        .WriteTo.Console()
        //.WriteTo.File("log.txt")
        .CreateLogger();

    try
    {
        var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
        await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
        var requestContent = Encoding.UTF8.GetString(buffer);
        context.Request.EnableBuffering();
        context.Request.Body.Position = 0;
        string postData = requestContent;

        log.Information("Hello, Serilog!");
        return Results.Ok();
    }catch(Exception ex)
    {
        log.Error("Hello, Serilog!");
        return Results.Ok();
    }

})
.WithName("GetLineMessage");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}