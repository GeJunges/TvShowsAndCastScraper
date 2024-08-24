using Refit;
using TvMaze.Repository.Entities;
using TvMazeScraper.Repository.Entities;

namespace TvMazeScraper.Service.ProviderApiClient
{
    public interface IProviderApi
    {
        [Get("/shows")]
        Task<ApiResponse<IEnumerable<TvShow>>> GetShowsPerPage(int? page = default, CancellationToken cancellationToken = default);

        [Get("/shows/{id}/cast")]
        Task<ApiResponse<IEnumerable<TvShowCast>>> GetCastPerShow(int id, CancellationToken cancellationToken = default);
    }
}
