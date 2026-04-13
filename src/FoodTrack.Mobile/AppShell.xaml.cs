namespace FoodTrack.Mobile;

/// <summary>
/// Defines the main tab-based mobile navigation shell.
/// </summary>
public partial class AppShell : Shell
{
    private static bool _routesRegistered;

    /// <summary>
    /// Initializes shell routes used by the mobile worker flows.
    /// </summary>
    public AppShell()
    {
        InitializeComponent();

        if (_routesRegistered)
        {
            return;
        }

        Routing.RegisterRoute(MobileRoutes.BatchDetail, typeof(Views.BatchDetailPage));
        Routing.RegisterRoute(MobileRoutes.ReceiveStock, typeof(Views.ReceiveStockPage));
        Routing.RegisterRoute(MobileRoutes.DispatchStock, typeof(Views.DispatchStockPage));
        Routing.RegisterRoute(MobileRoutes.AdjustStock, typeof(Views.AdjustStockPage));
        _routesRegistered = true;
    }
}
