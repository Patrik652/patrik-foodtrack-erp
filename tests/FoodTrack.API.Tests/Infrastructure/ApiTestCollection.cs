namespace FoodTrack.API.Tests;

/// <summary>
/// Disables parallel host startup for API integration tests over SQLite bootstrap.
/// </summary>
[CollectionDefinition(Name)]
public sealed class ApiTestCollection
{
    /// <summary>
    /// Shared collection name for API integration tests.
    /// </summary>
    public const string Name = "FoodTrack API Integration";
}
