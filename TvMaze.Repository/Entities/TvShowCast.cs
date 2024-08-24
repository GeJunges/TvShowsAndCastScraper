using TvMazeScraper.Repository.Entities;

namespace TvMaze.Repository.Entities
{
    public record TvShowCast
    {
        public Person Person { get; init; }
    }
}
