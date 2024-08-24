using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TvMazeScraper.Repository.Context;
using TvMazeScraper.Repository.Entities;

namespace TvMazeScraper.Repository
{
    public class TvShowsRespository : ITvShowsRespository
    {
        private readonly ILogger<TvShowsRespository> _logger;
        private TvMazeSraperDbContext DbContext { get; }

        private readonly DbConfiguration Configuration;

        public TvShowsRespository(ILogger<TvShowsRespository> logger,
            TvMazeSraperDbContext dbContext,
            IOptions<DbConfiguration> configuration)
        {
            _logger = logger;
            Configuration = configuration.Value;
            DbContext = dbContext;
        }

        public async Task<List<TvShow>> GetTvShows(int page, int take, CancellationToken cancellationToken = default) =>
            await DbContext.TvShows
                        .OrderBy(tv => tv.Id)
                        .Include(tvShow => tvShow.Cast.OrderByDescending(bd => bd.Birthday))
                        .Skip((page - 1) * take)
                        .Take(take)
                        .ToListAsync(cancellationToken);

        public async Task InsertOrUpdate(List<TvShow> tvShows, CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (var tvShow in tvShows)
                {
                    var existingTvShow = await DbContext.TvShows
                        .Include(item => item.Cast)
                        .FirstOrDefaultAsync(item => item.Id == tvShow.Id, cancellationToken);

                    if (existingTvShow is null)
                    {
                        existingTvShow = new TvShow { Id = tvShow.Id };
                        DbContext.TvShows.Add(existingTvShow);
                    }

                    existingTvShow.Name = tvShow.Name;

                    existingTvShow.Cast.Clear();
                    foreach (var tvShowCastPerson in tvShow.Cast.DistinctBy(member => member.Id))
                    {
                        var person = await DbContext.Persons.FindAsync([tvShowCastPerson.Id], cancellationToken);
                        if (person is null)
                        {
                            person = new Person { Id = tvShowCastPerson.Id };
                            DbContext.Persons.Add(person);
                        }

                        person.Name = tvShowCastPerson.Name;
                        person.Birthday = tvShowCastPerson.Birthday;

                        existingTvShow.Cast.Add(person);
                    }

                    await DbContext.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error inserting or updating shows and cast.", ex);
                throw;
            }
        }

        public async Task<TvShow> GetLatestTvShow(CancellationToken cancellationToken = default) =>
            await DbContext.TvShows
                .OrderByDescending(tv => tv.Id)
                .FirstOrDefaultAsync(cancellationToken);

    }
}
