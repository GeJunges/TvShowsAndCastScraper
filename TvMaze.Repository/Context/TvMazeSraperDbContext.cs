using Microsoft.EntityFrameworkCore;
using TvMazeScraper.Repository.Entities;

namespace TvMazeScraper.Repository.Context
{
    public class TvMazeSraperDbContext : DbContext
    {
        public TvMazeSraperDbContext(DbContextOptions<TvMazeSraperDbContext> options)
         : base(options) { }

        public DbSet<TvShow> TvShows { get; set; }
        public DbSet<Person> Persons { get; set; }
    }
}
