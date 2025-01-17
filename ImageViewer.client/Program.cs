using ImageViewer.client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace ImageViewer.client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5018") });

        builder.Services.AddBlazorBootstrap();

        builder.Services.AddScoped<DataAccessService>();
        builder.Services.AddSingleton<MainDisplayService>();

        await builder.Build().RunAsync();
    }
}
