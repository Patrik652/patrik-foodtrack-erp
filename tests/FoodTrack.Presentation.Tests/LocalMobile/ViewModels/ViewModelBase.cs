using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FoodTrack.Mobile.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    private string _errorMessage = string.Empty;
    private bool _isBusy;
    private string _statusMessage = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsBusy
    {
        get => _isBusy;
        protected set => SetProperty(ref _isBusy, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        protected set => SetProperty(ref _errorMessage, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        protected set => SetProperty(ref _statusMessage, value);
    }

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
        {
            return false;
        }

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected async Task ExecuteBusyAsync(Func<Task> action, string offlineFriendlyMessage)
    {
        ErrorMessage = string.Empty;
        StatusMessage = string.Empty;
        IsBusy = true;

        try
        {
            await action();
        }
        catch (HttpRequestException)
        {
            ErrorMessage = offlineFriendlyMessage;
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
