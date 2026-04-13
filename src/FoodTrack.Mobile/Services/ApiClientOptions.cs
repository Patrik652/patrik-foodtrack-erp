namespace FoodTrack.Mobile.Services;

/// <summary>
/// Configures the backend API address used by the mobile client.
/// </summary>
public sealed class ApiClientOptions
{
    /// <summary>
    /// Gets or sets the API base address.
    /// </summary>
    public Uri BaseAddress { get; init; } = new("http://10.0.2.2:5262/");
}
