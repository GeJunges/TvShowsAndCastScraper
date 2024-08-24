using AutoMapper;
using TvMazeScraper.Repository;
using TvMazeScraper.Service.Models;

namespace TvMazeScraper.Service
{
    public class TvShowService : ITvShowService
    {

        public const int StartPageNumber = 1;
        public const int ItensPerPage = 25;

        private ITvShowsRespository Repository { get; }
        private readonly IMapper Mapper;

        public TvShowService(ITvShowsRespository repository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
        }

        public async Task<List<TvShowResponse>> GetAllTvShows(int page, int itensPerPage, CancellationToken cancellationToken = default)
        {

            page = page < StartPageNumber ? StartPageNumber : page;
            itensPerPage = itensPerPage <= 0 ? ItensPerPage : itensPerPage;

            var shows = await Repository.GetTvShows(page, itensPerPage, cancellationToken);

            return Mapper.Map<List<TvShowResponse>>(shows);
        }
    }
}
