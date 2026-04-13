using FoodTrack.Mobile.Services;

namespace FoodTrack.Mobile;

/// <summary>
/// Application bootstrapper for the warehouse worker mobile shell.
/// </summary>
public partial class App : Application
{
    private readonly AuthSessionService _authSessionService;

    /// <summary>
    /// Initializes the mobile application root and subscribes to auth-session transitions.
    /// </summary>
    public App()
    {
        InitializeComponent();
        _authSessionService = ServiceHelper.GetRequiredService<AuthSessionService>();
        _authSessionService.SessionChanged += HandleSessionChanged;
        RefreshRoot();
    }

    /// <summary>
    /// Refreshes the visible root page according to the operator session.
    /// </summary>
    public void RefreshRoot()
    {
        MainPage = _authSessionService.IsAuthenticated
            ? new AppShell()
            : new NavigationPage(new Views.LoginPage());
    }

    private void HandleSessionChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(RefreshRoot);
    }
}
