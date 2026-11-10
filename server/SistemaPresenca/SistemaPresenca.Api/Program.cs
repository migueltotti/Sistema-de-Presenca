using SistemaPresenca.Api.Extensions;
using SistemaPresenca.Api.Handlers;
using SistemaPresenca.CrossCutting.Extensions;
using SistemaPresenca.CrossCutting.Models;

var builder = WebApplication.CreateBuilder(args);

var applicationSettings = builder.Configuration.GetApplicationSettings(builder.Environment);

builder.Services
    .AddSingleton<ISettings>(applicationSettings)
    .AddControllers(ControllerExtensions.ConfigureMvcOptions)
    .ConfigureApiBehaviorOptions(ControllerExtensions.ConfigureApiBehaviorOptions);

builder.Services
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddProblemDetails()
    .AddDatabase(applicationSettings.PostgresSettings)
    .ConfigureAuthentication(applicationSettings.JwtSettings, applicationSettings.ApiKeySettings)
    .AddRepositories()
    .AddServices()
    .AddMediator()
    .AddValidators()
    .AddMqtt(applicationSettings.MqttSettings)
    .AddApiSpecification()
    .AddEndpointsApiExplorer();

var app = builder.Build();

app.MapOpenApi();
app.UseSpecification();

app
    //.UseHttpsRedirection()
    .UseExceptionHandler()
    .UseAuthentication()
    .UseAuthorization();

app.MapControllers();

await app.RunAsync();
