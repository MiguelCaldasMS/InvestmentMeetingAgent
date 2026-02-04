using InvestmentMeeting.WebApp;
using Microsoft.Agents.Core;
using Microsoft.Agents.Hosting.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

// Register the agent services
builder.Services.AddTransient<IBot, MeetingAgent>();
builder.Services.AddCloudAdapter();

var app = builder.Build();

app.UseHttpsRedirection();

// Map the agent's message endpoint
app.MapPost("/api/messages", async (HttpRequest req, IBotHttpAdapter adapter, IBot agent) =>
{
    await adapter.ProcessAsync(req, req.HttpContext.Response, agent);
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () => {
  var forecast = Enumerable.Range(1, 5).Select(index =>
      new WeatherForecast
      (
          DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
          Random.Shared.Next(-20, 55),
          summaries[Random.Shared.Next(summaries.Length)]
      ))
      .ToArray();
  return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary) {
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
