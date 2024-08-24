using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TvMazeScraper.Service;

namespace TvMazeScraper.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private IScraperService Service { get; set; }

    public Worker(ILogger<Worker> logger, IScraperService scraperService)
    {
        _logger = logger;
        Service = scraperService;
    }
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        int pageNumber = await Service.GetStartingPage();

        _logger.LogInformation("Worker running started at: {time}", DateTimeOffset.Now);

        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time} and page {page}", DateTimeOffset.Now, pageNumber);

            await Service.GetAllTvShows(pageNumber, cancellationToken);

            pageNumber++;
        }

        _logger.LogInformation("Worker running ended at: {time}", DateTimeOffset.Now);
    }
}
