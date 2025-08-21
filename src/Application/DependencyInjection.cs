using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedCookbook.Application.Common.Behaviours;

namespace SharedCookbook.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            configuration.AddOpenRequestPreProcessor(typeof(LoggingBehaviour<>));
            configuration.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            configuration.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
            configuration.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            configuration.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
        });
    }
}
