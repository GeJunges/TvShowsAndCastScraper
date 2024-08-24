using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.RateLimit;
using Polly.Retry;
using System.Net;
using TvMazeScraper.Repository;
using TvMazeScraper.Repository.Entities;
using TvMazeScraper.Service.ProviderApiClient;
using TvMazeScraper.Service.Settings;

namespace TvMazeScraper.Service;

public class ScraperService : IScraperService
{
    private readonly ILogger<ScraperService> _logger;
    private ProviderApiConfiguration ApiConfigurations { get; }
    private IProviderApi ProviderApi { get; set; }
    private ITvShowsRespository Repository { get; }

    private static AsyncRetryPolicy RetryPolicy => Policy
                .Handle<HttpRequestException>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    private static AsyncRateLimitPolicy RateLimitPolicy => Policy.RateLimitAsync(10, TimeSpan.FromSeconds(1));

    public ScraperService(ILogger<ScraperService> logger,
        IOptions<ProviderApiConfiguration> apiConfigurations,
        IProviderApi providerApi,
        ITvShowsRespository repository)
    {
        ApiConfigurations = apiConfigurations.Value;
        _logger = logger;
        ProviderApi = providerApi;
        Repository = repository;
    }

    public async Task<int> GetStartingPage()
    {
        var latestTvShow = await Repository.GetLatestTvShow();

        return latestTvShow is null ? 0 : latestTvShow.Id / ApiConfigurations.ShowsPerPage;
    }

    public async Task GetAllTvShows(int pageNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await RetryPolicy.ExecuteAsync(() => ProviderApi.GetShowsPerPage(pageNumber, cancellationToken));
            var tvShows = response.Content?.ToList();

            if (tvShows?.Count == 0)
            {
                throw new OperationCanceledException("No more shows found, stopping operation.", cancellationToken);
            }

            var tvShowsWithCast = await ExtractCastPerShow(tvShows!, cancellationToken);

            await Repository.InsertOrUpdate(tvShowsWithCast, cancellationToken);

        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching shows for page number: {pageNumber}", ex);
            throw;
        }
    }

    private async Task<List<TvShow>> ExtractCastPerShow(List<TvShow> tvShows, CancellationToken cancellationToken)
    {

        var tasks = tvShows.Select(async tvShow =>
        {
            try
            {
                var result = await RateLimitPolicy.ExecuteAsync(() =>
                                       RetryPolicy.ExecuteAsync(() => ProviderApi.GetCastPerShow(tvShow.Id, cancellationToken)));


                if (result.StatusCode == HttpStatusCode.NotFound || result.Content is null)
                {
                    _logger.LogWarning("Cannot find cast for show {ShowId}.", tvShow.Id);
                    return;
                }

                var tvShowCast = result.Content?.ToList();

                if (tvShowCast?.Count > 0)
                {
                    var cast = tvShowCast.Select(p => p.Person).ToList();
                    tvShow.Cast.AddRange(cast);
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching cast for tvShow id: {tvShow.Id}, name: {tvShow.Name}", ex);
                throw;
            }
        }).ToList();

        await Task.WhenAll(tasks);

        return tvShows;
    }
}
