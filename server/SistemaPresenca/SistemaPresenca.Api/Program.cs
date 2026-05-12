using SistemaPresenca.CrossCutting.Models;
using SistemaPresenca.CrossCutting.Extensions;

var builder = WebApplication.CreateBuilder(args);

var applicationSettings = builder.Configuration.GetApplicationSettings(builder.Environment);

builder.Services
    .AddSingleton<ISettings>(applicationSettings)
    .AddControllers();

builder.Services
    .AddDatabase(applicationSettings.PostgresSettings);

var app = builder.Build();

app.MapOpenApi();

app.UseHttpsRedirection()
    .UseAuthorization();

app.MapControllers();

await app.RunAsync();
