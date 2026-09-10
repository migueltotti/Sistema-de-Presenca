using Microsoft.AspNetCore.Mvc;
using SistemaPresenca.Api.Filters;

namespace SistemaPresenca.Api.Extensions;

public static class ControllerExtensions
{
    public static void ConfigureMvcOptions(MvcOptions options)
    {
        options.Filters.Add<ValidationsFilterAttribute>();
    }

    public static void ConfigureApiBehaviorOptions(ApiBehaviorOptions options)
    {
        options.SuppressModelStateInvalidFilter = true;
        options.SuppressInferBindingSourcesForParameters = true;
    }
}
