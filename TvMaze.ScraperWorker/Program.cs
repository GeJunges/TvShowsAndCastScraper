using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;
using TvMazeScraper.Repository;
using TvMazeScraper.Repository.Context;
using TvMazeScraper.Service;
using TvMazeScraper.Service.ProviderApiClient;
using TvMazeScraper.Service.Settings;
using TvMazeScraper.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();

        builder.Services.AddAutoMapper(typeof(TvShowProfile));

        ConfigureServices(builder);

        var host = builder.Build();
        host.Run();
    }

    private static void ConfigureServices(HostApplicationBuilder builder)
    {
        //Register AppSettings classes
        var apiConfigs = builder.Configuration.GetSection(nameof(ProviderApiConfiguration));
        builder.Services.Configure<ProviderApiConfiguration>(apiConfigs);
        var baseUrl = apiConfigs.Get<ProviderApiConfiguration>()?.BaseUrl;

        var dbConfigurations = builder.Configuration.GetSection(nameof(DbConfiguration));
        builder.Services.Configure<DbConfiguration>(dbConfigurations);
        var connectionString = dbConfigurations.Get<DbConfiguration>()?.ConnectionString;

        // Configure DbContext with SQLite using the connection string
        builder.Services.AddDbContext<TvMazeSraperDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        builder.Services.AddScoped<TvMazeSraperDbContext>();

        //Register Refit
        builder.Services.AddRefitClient<IProviderApi>()
             .ConfigureHttpClient(client =>
             {
                 client.BaseAddress = new(baseUrl);
                 client.DefaultRequestHeaders.Add("User-Agent", "IProviderApi");
             });

        //Register Services
        builder.Services.AddScoped<IScraperService, ScraperService>();
        builder.Services.AddScoped<ITvShowService, TvShowService>();

        //Register Repositories 
        builder.Services.AddScoped<ITvShowsRespository, TvShowsRespository>();
    }
}