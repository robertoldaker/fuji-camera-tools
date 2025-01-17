
using ImageViewer.client;
using ImageViewer.server.Services;
using ImageViewer.shared;
using Config = ImageViewer.server.Services.Config;

namespace ImageViewer.server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // CORS
        var corsPolicyName = "allowAll";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(corsPolicyName,
                                  builder => {
                                      builder.WithOrigins("http://localhost:5016")
                                             .AllowCredentials()
                                             .AllowAnyHeader()
                                             .AllowAnyMethod();
                                  });
        });


        // add built in services
        AddAppServices(builder);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseWebAssemblyDebugging();
        }

        app.UseCors(corsPolicyName);
        app.UseAuthorization();

        //
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();
        app.UseRouting();

        //
        app.MapRazorPages();
        //??app.MapControllers();
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action}");
        app.MapFallbackToFile("index.html");

        app.Run();
    }

    private static void AddAppServices(WebApplicationBuilder builder)
    {
        var config = Config.CreateConfig();
        builder.Services.AddSingleton<Config>(config);
        builder.Services.AddSingleton<ImageLibrary>(new ImageLibrary(config));
    }
}
