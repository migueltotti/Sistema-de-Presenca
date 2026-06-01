using Mattioli.Configurations.Extensions.Controllers;
using Mattioli.Configurations.Extensions.Handlers;
using SistemaPresenca.CrossCutting.Extensions;
using SistemaPresenca.CrossCutting.Models;

var builder = WebApplication.CreateBuilder(args);

var applicationSettings = builder.Configuration.GetApplicationSettings(builder.Environment);

builder.Services
    .AddSingleton<ISettings>(applicationSettings)
    .AddControllers(ControllerExtensions.ConfigureMvcOptions)
    .AddNewtonsoftJson(ControllerExtensions.ConfigureNewtonsoftJson)
    .ConfigureApiBehaviorOptions(ControllerExtensions.ConfigureApiBehaviorOptions);

builder.Services
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddProblemDetails()
    .AddDatabase(applicationSettings.PostgresSettings)
    .AddRepositories()
    .AddMediator()
    .AddApiSpecification()
    .AddEndpointsApiExplorer();

var app = builder.Build();

app.MapOpenApi();
app.UseSpecification();

app
    .UseHttpsRedirection()
    .UseAuthorization();

app.MapControllers();

await app.RunAsync();
