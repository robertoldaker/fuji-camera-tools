using ImageViewer.client.Services;
using Microsoft.AspNetCore.Components;
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

#if DEBUG
        builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5018") });
#else
        builder.Services.AddSingleton(sp =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
        });
#endif
        
        builder.Services.AddBlazorBootstrap();

        builder.Services.AddSingleton<DataAccessService>();
        builder.Services.AddSingleton<MainDisplayService>();
        builder.Services.AddSingleton<ImageLibraryService>();
        builder.Services.AddSingleton<ModalsService>();

        await builder.Build().RunAsync();
    }
}
