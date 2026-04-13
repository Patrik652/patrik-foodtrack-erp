using Microsoft.Extensions.DependencyInjection;

namespace FoodTrack.Mobile;

/// <summary>
/// Exposes the MAUI service provider to Shell-instantiated pages and views.
/// </summary>
public static class ServiceHelper
{
    private static IServiceProvider? _services;

    /// <summary>
    /// Stores the MAUI application service provider after the host is built.
    /// </summary>
    public static void Initialize(IServiceProvider services)
    {
        _services = services;
    }

    /// <summary>
    /// Resolves a required service from the active MAUI service provider.
    /// </summary>
    public static T GetRequiredService<T>() where T : notnull
    {
        return (_services ?? throw new InvalidOperationException("The MAUI service provider has not been initialized yet."))
            .GetRequiredService<T>();
    }
}
