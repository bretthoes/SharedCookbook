using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SharedCookbook.Application.Common.Mediator;

/// <summary>
/// Extensions for registering mediator services
/// </summary>
public static class MediatorExtensions
{
    /// <summary>
    /// Add mediator services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Configuration action</param>
    public static void AddMediator(this IServiceCollection services, Action<MediatorConfiguration> configure)
    {
        var configuration = new MediatorConfiguration();
        configure(configuration);

        // Register the mediator
        services.AddScoped<IMediator, Mediator>();
        services.AddScoped<ISender>(sp => sp.GetRequiredService<IMediator>());
        services.AddScoped<IPublisher>(sp => sp.GetRequiredService<IMediator>());

        // Register handlers from assemblies
        foreach (var assembly in configuration.Assemblies)
        {
            RegisterHandlersFromAssembly(services, assembly);
        }

        // Register open behaviors
        foreach (var (behaviorType, serviceLifetime) in configuration.Behaviors)
        {
            services.Add(new ServiceDescriptor(
                typeof(IPipelineBehavior<,>),
                behaviorType,
                serviceLifetime));
        }

        // Register open pre-processors
        foreach (var (preProcessorType, serviceLifetime) in configuration.PreProcessors)
        {
            services.Add(new ServiceDescriptor(
                typeof(IRequestPreProcessor<>),
                preProcessorType,
                serviceLifetime));
        }
    }

    private static void RegisterHandlersFromAssembly(IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false });

        foreach (var type in types)
        {
            // Register request handlers with response type (IRequestHandler<TRequest, TResponse>)
            var requestHandlerInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

            foreach (var handlerInterface in requestHandlerInterfaces)
            {
                services.AddScoped(handlerInterface, type);
            }

            // Register request handlers without response type (IRequestHandler<TRequest>)
            var requestHandlerInterfacesNoResponse = type.GetInterfaces()
                .Where(i => i.IsGenericType && 
                            i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) &&
                            i.GenericTypeArguments.Length == 1);

            foreach (var handlerInterface in requestHandlerInterfacesNoResponse)
            {
                services.AddScoped(handlerInterface, type);
            }

            // Register notification handlers
            var notificationHandlerInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>));

            foreach (var handlerInterface in notificationHandlerInterfaces)
            {
                services.AddScoped(handlerInterface, type);
            }
        }
    }
}

/// <summary>
/// Configuration for the mediator
/// </summary>
public sealed class MediatorConfiguration
{
    internal List<Assembly> Assemblies { get; } = [];
    internal List<(Type Type, ServiceLifetime Lifetime)> Behaviors { get; } = [];
    internal List<(Type Type, ServiceLifetime Lifetime)> PreProcessors { get; } = [];

    /// <summary>
    /// Register all handlers from the specified assembly
    /// </summary>
    /// <param name="assembly">The assembly to scan</param>
    public void RegisterServicesFromAssembly(Assembly assembly)
    {
        Assemblies.Add(assembly);
    }

    /// <summary>
    /// Add an open generic pipeline behavior
    /// </summary>
    /// <param name="behaviorType">The open generic behavior type</param>
    /// <param name="lifetime">Service lifetime (default: Scoped)</param>
    public void AddOpenBehavior(Type behaviorType, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        Behaviors.Add((behaviorType, lifetime));
    }

    /// <summary>
    /// Add an open generic request pre-processor
    /// </summary>
    /// <param name="preProcessorType">The open generic pre-processor type</param>
    /// <param name="lifetime">Service lifetime (default: Scoped)</param>
    public void AddOpenRequestPreProcessor(Type preProcessorType, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        PreProcessors.Add((preProcessorType, lifetime));
    }
}
