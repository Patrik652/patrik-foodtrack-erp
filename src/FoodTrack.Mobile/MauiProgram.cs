using FoodTrack.Mobile.Services;
using FoodTrack.Mobile.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FoodTrack.Mobile;

/// <summary>
/// Configures the mobile application service graph.
/// </summary>
public static class MauiProgram
{
    /// <summary>
    /// Creates the MAUI host used by the warehouse mobile app.
    /// </summary>
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        var defaultBaseAddress = DeviceInfo.Current.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5262/"
            : "http://127.0.0.1:5262/";

        builder.Services.AddSingleton(new ApiClientOptions
        {
            BaseAddress = new Uri(defaultBaseAddress)
        });
        builder.Services.AddSingleton<AuthSessionService>();
        builder.Services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<ApiClientOptions>();
            return new HttpClient { BaseAddress = options.BaseAddress };
        });
        builder.Services.AddSingleton<IApiService, ApiService>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<ProductListViewModel>();
        builder.Services.AddTransient<BatchDetailViewModel>();
        builder.Services.AddTransient<ReceiveStockViewModel>();
        builder.Services.AddTransient<DispatchStockViewModel>();
        builder.Services.AddTransient<AdjustStockViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        ServiceHelper.Initialize(app.Services);
        return app;
    }
}
