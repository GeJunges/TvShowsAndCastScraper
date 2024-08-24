using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TvMazeScraper.Repository;
using TvMazeScraper.Repository.Context;
using TvMazeScraper.Repository.Entities;

namespace TvMaze.IntegrationTests.TvMaze.Repository
{
    public class TvShowsRespositoryTests
    {
        private readonly Mock<ILogger<TvShowsRespository>> _loggerMock;
        private readonly TvMazeSraperDbContext _dbContext;
        private readonly TvShowsRespository _repository;

        public TvShowsRespositoryTests()
        {
            _loggerMock = new Mock<ILogger<TvShowsRespository>>();

            var options = new DbContextOptionsBuilder<TvMazeSraperDbContext>()
                .UseInMemoryDatabase(databaseName: "amaze-test.db")
                .Options;

            _dbContext = new TvMazeSraperDbContext(options);
            _dbContext.Database.EnsureCreated();

            var mockConfiguration = new DbConfiguration { ConnectionString = "InMemoryDatabase" };
            var mockOptions = Options.Create(mockConfiguration);

            _repository = new TvShowsRespository(
               _loggerMock.Object,
                _dbContext,
                mockOptions
            );
        }

        [Fact]
        public async Task GetTvShows_ShouldReturnOrderedTvShows()
        {
            // Arrange
            var tvShows = new List<TvShow>
            {
                new TvShow { Id = 1, Name = "Show 1", Cast = new List<Person> { new Person { Id = 1, Name = "Actor 1", Birthday = new DateOnly(1980, 1, 1) } } },
                new TvShow { Id = 2, Name = "Show 2", Cast = new List<Person> { new Person { Id = 2, Name = "Actor 2", Birthday = new DateOnly(1990, 1, 1) } } }
            };

            await _dbContext.TvShows.AddRangeAsync(tvShows);
            await _dbContext.SaveChangesAsync();

            // Act
            var actual = await _repository.GetTvShows(1, 10);

            // Assert
            actual.Count.Should().Be(2);
            actual.First().Name.Should().Be("Show 1");
            actual.Last().Name.Should().Be("Show 2");
        }

        [Fact]
        public async Task InsertOrUpdate_AddsNewTvShow()
        {
            // Arrange
            var tvShowToAdd = new TvShow { Id = 3, Name = "New Show", Cast = new List<Person> { new Person { Id = 3, Name = "New Actor", Birthday = new DateOnly(1985, 5, 5) } } };

            // Act
            await _repository.InsertOrUpdate(new List<TvShow> { tvShowToAdd });

            // Assert
            var addedTvShow = await _dbContext.TvShows.FindAsync(3);
            Assert.NotNull(addedTvShow);
            Assert.Equal("New Show", addedTvShow.Name);
        }

        [Fact]
        public async Task InsertOrUpdate_UpdatesExistingTvShow()
        {
            // Arrange
            var existingTvShow = new TvShow { Id = 4, Name = "Old Show", Cast = new List<Person> { new Person { Id = 4, Name = "Old Actor", Birthday = new DateOnly(1980, 1, 1) } } };
            await _dbContext.TvShows.AddAsync(existingTvShow);
            await _dbContext.SaveChangesAsync();

            var updatedTvShow = new TvShow { Id = 4, Name = "Updated Show", Cast = new List<Person> { new Person { Id = 4, Name = "Updated Actor", Birthday = new DateOnly(1980, 1, 1) } } };

            // Act
            await _repository.InsertOrUpdate(new List<TvShow> { updatedTvShow });
            var actual = await _dbContext.TvShows.Include(ts => ts.Cast).FirstOrDefaultAsync(ts => ts.Id == 4);

            // Assert
            actual.Name.Should().Be("Updated Show");
            actual.Cast.Count.Should().Be(1);
            actual.Cast.First().Name.Should().Be("Updated Actor");
        }

        [Fact]
        public async Task GetLatestTvShow_ReturnsLatestTvShow()
        {
            // Arrange
            var latestTvShow = new TvShow { Id = 5, Name = "Latest Show" };
            await _dbContext.TvShows.AddAsync(latestTvShow);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetLatestTvShow();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(5);
        }

    }
}
