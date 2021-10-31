using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Cooking.Web;

/// <summary>
/// Web application Program class.
/// </summary>
public static class Program
{
    /// <summary>
    /// Entry point for application.
    /// </summary>
    /// <param name="args">Arguments for application.</param>
    public static void Main(string[] args) => CreateWebHostBuilder(args)
                                                .Build()
                                                .Run();

    /// <summary>
    /// Create web host builder.
    /// </summary>
    /// <param name="args">Arguments for application.</param>
    /// <returns>Web host builder.</returns>
    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
}
