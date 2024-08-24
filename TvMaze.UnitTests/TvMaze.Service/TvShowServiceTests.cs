using AutoMapper;
using TvMazeScraper.Repository.Entities;
using TvMazeScraper.Repository;
using TvMazeScraper.Service.Models;
using TvMazeScraper.Service;
using TvMazeScraper.Service.Settings;

namespace TvMaze.UnitTests.TvMaze.Service
{
    public class TvShowServiceTests
    {
        private readonly Mock<ITvShowsRespository> _repositoryMock;
        private readonly IMapper _mapper;
        private readonly TvShowService _tvShowService;
        private readonly Fixture _fixture = new();

        public TvShowServiceTests()
        {
            _fixture.Customize(new AutoFixtureCustomization());
            _repositoryMock = new Mock<ITvShowsRespository>();

            _mapper = InitializeAutoMapper();

            _tvShowService = new TvShowService(_repositoryMock.Object, _mapper);
        }

        private IMapper InitializeAutoMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TvShowProfile>();
            });

            return mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task GetAllTvShows_ShouldReturn_ListOfTvShowResponse()
        {
            // Arrange
            var page = 1;
            var itensPerPage = 10;
            var tvShows = _fixture.Create<List<TvShow>>();

            _repositoryMock.Setup(r => r.GetTvShows(page, itensPerPage, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tvShows);

            // Act
            var actual = await _tvShowService.GetAllTvShows(page, itensPerPage);

            // Assert
            actual.Should().BeOfType<List<TvShowResponse>>();
        }

        [Fact]
        public async Task GetAllTvShows_ShouldUseDefaultStartPageNumber_WhenPageIsLessThanStartPageNumber()
        {
            // Arrange
            var page = 0;
            var itensPerPage = 10;

            _repositoryMock.Setup(r => r.GetTvShows(TvShowService.StartPageNumber, itensPerPage, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TvShow>());

            // Act
            var result = await _tvShowService.GetAllTvShows(page, itensPerPage);

            // Assert
            _repositoryMock.Verify(r => r.GetTvShows(TvShowService.StartPageNumber, itensPerPage, It.IsAny<CancellationToken>()), Times.Once);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task GetAllTvShows_ShouldUseDefaultItensPerPage_WhenItensPerPageIsLessThanOrEqualToZero( int itensPerPage)
        {
            // Arrange
            var page = 1;

            _repositoryMock.Setup(r => r.GetTvShows(page, TvShowService.ItensPerPage, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TvShow>());

            // Act
            var result = await _tvShowService.GetAllTvShows(page, itensPerPage);

            // Assert
            _repositoryMock.Verify(r => r.GetTvShows(page, TvShowService.ItensPerPage, It.IsAny<CancellationToken>()), Times.Once);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllTvShows_ShouldHandleCancellation()
        {
            // Arrange
            var page = 1;
            var itensPerPage = 10;
            var cancellationToken = new CancellationToken(canceled: true);

            _repositoryMock.Setup(r => r.GetTvShows(page, itensPerPage, cancellationToken))
                .ThrowsAsync(new TaskCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() => _tvShowService.GetAllTvShows(page, itensPerPage, cancellationToken));
        }
    }
}

