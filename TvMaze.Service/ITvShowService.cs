using TvMazeScraper.Service.Models;

namespace TvMazeScraper.Service
{
    public interface ITvShowService
    {
        Task<List<TvShowResponse>> GetAllTvShows(int page, int itensPerPage, CancellationToken cancellationToken = default);
    }
}