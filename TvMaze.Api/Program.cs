using TvMazeScraper.Repository.Context;
using TvMazeScraper.Service.Settings;
using TvMazeScraper.Repository;
using TvMazeScraper.Service;
using Microsoft.EntityFrameworkCore;

namespace TvMazeScraper;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        ConfigureServices(builder);

        builder.Services.AddControllers();

        builder.Services.AddAutoMapper(typeof(TvShowProfile));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        //Register AppSettings classes
        var dbConfigurations = builder.Configuration.GetSection(nameof(DbConfiguration));
        builder.Services.Configure<DbConfiguration>(dbConfigurations);
        var connectionString = dbConfigurations.Get<DbConfiguration>()?.ConnectionString;

        // Configure DbContext with SQLite using the connection string
        builder.Services.AddDbContext<TvMazeSraperDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        //Register Services
        builder.Services.AddScoped<ITvShowService, TvShowService>();

        //Register Repositories 
        builder.Services.AddScoped<ITvShowsRespository, TvShowsRespository>();
    }
}