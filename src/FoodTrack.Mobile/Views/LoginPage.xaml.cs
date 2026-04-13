using FoodTrack.Mobile.ViewModels;

namespace FoodTrack.Mobile.Views;

/// <summary>
/// Login page for warehouse operator authentication.
/// </summary>
public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public LoginPage()
    {
        InitializeComponent();
        _viewModel = ServiceHelper.GetRequiredService<LoginViewModel>();
        BindingContext = _viewModel;
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        await _viewModel.LoginAsync();
    }
}
