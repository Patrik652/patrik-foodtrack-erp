using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace FoodTrack.API.Tests;

/// <summary>
/// Boots the API against an isolated SQLite database for integration tests.
/// </summary>
public sealed class FoodTrackApiFactory : WebApplicationFactory<Program>
{
    private readonly string _tempDirectory = Path.Combine(Path.GetTempPath(), $"foodtrack-api-tests-{Guid.NewGuid():N}");
    private string DatabasePath => Path.Combine(_tempDirectory, "foodtrack-tests.db");

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Directory.CreateDirectory(_tempDirectory);

        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:FoodTrack"] = $"Data Source={DatabasePath}"
            });
        });
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
        {
            return;
        }

        try
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, recursive: true);
            }
        }
        catch (IOException)
        {
            // Ignore best-effort cleanup issues in temp test files.
        }
        catch (UnauthorizedAccessException)
        {
            // Ignore best-effort cleanup issues in temp test files.
        }
    }
}
