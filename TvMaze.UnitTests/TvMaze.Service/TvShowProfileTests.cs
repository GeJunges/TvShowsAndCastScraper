using AutoMapper;
using TvMazeScraper.Repository.Entities;
using TvMazeScraper.Service.Models;
using TvMazeScraper.Service.Settings;

namespace TvMaze.UnitTests.TvMaze.Service
{
    public class TvShowProfileTests
    {
        private readonly IMapper _mapper;

        public TvShowProfileTests()
        {
            // Arrange
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<TvShowProfile>());
            _mapper = configuration.CreateMapper();
        }

        [Fact]
        public void ShouldHaveValidConfiguration()
        {
            // Act & Assert
            _mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [Fact]
        public void ShouldMapPersonToPersonResponse()
        {
            // Arrange
            var person = new Person
            {
                Id = 1,
                Name = "John Doe",
                Birthday = new DateOnly(1980, 1, 1)
            };

            // Act
            var personResponse = _mapper.Map<PersonResponse>(person);

            // Assert
            Assert.Equal(person.Id, personResponse.Id);
            Assert.Equal(person.Name, personResponse.Name);
            Assert.Equal(person.Birthday, personResponse.Birthday);
        }

        [Fact]
        public void ShouldMapTvShowToTvShowResponse()
        {
            // Arrange
            var tvShow = new TvShow
            {
                Id = 1,
                Name = "Sample Show",
                Cast = new List<Person>
            {
                new Person
                {
                    Id = 2,
                    Name = "Jane Doe",
                    Birthday = new DateOnly(1990, 5, 15)
                }
            }
            };

            // Act
            var tvShowResponse = _mapper.Map<TvShowResponse>(tvShow);

            // Assert
            Assert.Equal(tvShow.Id, tvShowResponse.Id);
            Assert.Equal(tvShow.Name, tvShowResponse.Name);
            Assert.Single(tvShowResponse.Cast);
            Assert.Equal(tvShow.Cast.First().Id, tvShowResponse.Cast.First().Id);
            Assert.Equal(tvShow.Cast.First().Name, tvShowResponse.Cast.First().Name);
            Assert.Equal(tvShow.Cast.First().Birthday, tvShowResponse.Cast.First().Birthday);
        }
    }
}
