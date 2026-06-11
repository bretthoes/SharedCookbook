using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedCookbook.Application.Common.Behaviours;
using SharedCookbook.Application.Notifications.FanOut;
using SharedCookbook.Application.Users;

namespace SharedCookbook.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<INotificationFanOut, NotificationFanOut>();
        builder.Services.AddScoped<IDisplayNamePropagation, DisplayNamePropagation>();
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Services.AddMediator(configuration =>
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
