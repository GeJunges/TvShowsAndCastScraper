namespace TvMazeScraper.Service
{
    public interface IScraperService
    {
        Task<int> GetStartingPage();

        Task GetAllTvShows(int pageNumber, CancellationToken cancellationToken = default);
    }
}
