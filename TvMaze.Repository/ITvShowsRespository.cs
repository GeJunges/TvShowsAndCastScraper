using TvMazeScraper.Repository.Entities;

namespace TvMazeScraper.Repository
{
    public interface ITvShowsRespository
    {
        Task<TvShow> GetLatestTvShow(CancellationToken cancellationToken = default);

        Task<List<TvShow>> GetTvShows(int page, int take, CancellationToken cancellationToken = default);

        Task InsertOrUpdate(List<TvShow> tvShows, CancellationToken cancellationToken = default);
    }

}