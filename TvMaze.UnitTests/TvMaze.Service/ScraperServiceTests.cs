using Microsoft.Extensions.Options;
using Refit;
using TvMazeScraper.Repository.Entities;
using TvMazeScraper.Repository;
using TvMazeScraper.Service.ProviderApiClient;
using TvMazeScraper.Service.Settings;
using TvMazeScraper.Service;
using TvMazeScraper.UnitTests;
using TvMaze.Repository.Entities;

namespace TvMaze.UnitTests.TvMaze.Service
{
    public class ScraperServiceTests
    {
        private readonly Mock<ILogger<ScraperService>> _loggerMock;
        private readonly Mock<IProviderApi> _providerApiMock;
        private readonly Mock<ITvShowsRespository> _repositoryMock;
        private readonly ScraperService _scraperService;
        private Fixture _fixture = new();

        public ScraperServiceTests()
        {
            _fixture.Customize(new AutoFixtureCustomization());

            _loggerMock = _fixture.Freeze<Mock<ILogger<ScraperService>>>();
            _providerApiMock = _fixture.Freeze<Mock<IProviderApi>>();
            _repositoryMock = new Mock<ITvShowsRespository>();

            var apiConfigMock = Options.Create(new ProviderApiConfiguration
            {
                ShowsPerPage = 14
            });

            _scraperService = new ScraperService(
                _loggerMock.Object,
                apiConfigMock,
                _providerApiMock.Object,
                _repositoryMock.Object);
        }

        [Fact]
        public async Task GetAllTvShows_ShouldCallInsertOrUpdateTvShows_IfSuccessfulExecution()
        {
            // Arrange
            var tvShowWithoutCast = _fixture.Build<TvShow>().Without(x => x.Cast).Create();
            var tvShows = new List<TvShow> { tvShowWithoutCast };
            var cast = _fixture.Create<List<TvShowCast>>();
            var expected = cast.Select(s => s.Person).ToList();

            var apiShowsResponse = new ApiResponse<IEnumerable<TvShow>>(new HttpResponseMessage(), tvShows, null);
            var apiCastResponse = new ApiResponse<IEnumerable<TvShowCast>>(new HttpResponseMessage(), cast, null);

            _providerApiMock.Setup(x => x.GetShowsPerPage(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiShowsResponse);

            _providerApiMock.Setup(x => x.GetCastPerShow(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiCastResponse);

            // Act
            await _scraperService.GetAllTvShows(1);

            // Assert
            _repositoryMock.Verify(x => x.InsertOrUpdate(It.Is<List<TvShow>>(tvShows => tvShows.Count() == 1), It.IsAny<CancellationToken>()), Times.Once);
            _repositoryMock.Verify(mock => mock.InsertOrUpdate(It.Is<List<TvShow>>(tvShow => tvShow.First().Cast.SequenceEqual(expected)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllTvShows_ShouldThrowOperationCanceledException_IfNoTvShowsFound()
        {
            // Arrange
            var pageNumber = 1;
            var apiResponse = new ApiResponse<IEnumerable<TvShow>>(new HttpResponseMessage(), new List<TvShow>(), null);

            _providerApiMock.Setup(x => x.GetShowsPerPage(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => _scraperService.GetAllTvShows(pageNumber));
        }

        [Fact]
        public async Task GetAllTvShows_ShouldLogError_IfExceptionThrown()
        {
            // Arrange
            var pageNumber = 1;
            var exception = new Exception("Test exception");
            var expected = $"Error fetching shows for page number: {pageNumber}";

            _providerApiMock.Setup(x => x.GetShowsPerPage(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            // Act & Assert
            var actual = await Assert.ThrowsAsync<Exception>(() => _scraperService.GetAllTvShows(pageNumber));

            actual.Message.Should().Be(exception.Message);
            _loggerMock.VerifyLogMessage(LogLevel.Error, expected, Times.Once());
        }

        [Fact]
        public async Task GetAllTvShows_ShouldLogErrorAndRethrow_IfExtractCastPerShow_ThrownException()
        {
            // Arrange
            var tvShowWithoutCast = _fixture.Build<TvShow>().Without(x => x.Cast).Create();
            var exception = new Exception("Test exception");
            var expected = $"Error fetching cast for tvShow id: {tvShowWithoutCast.Id}, name: {tvShowWithoutCast.Name}";

            var tvShows = new List<TvShow> { tvShowWithoutCast };

            var apiShowsResponse = new ApiResponse<IEnumerable<TvShow>>(new HttpResponseMessage(), tvShows, null);
            _providerApiMock.Setup(x => x.GetShowsPerPage(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiShowsResponse);

            _providerApiMock.Setup(x => x.GetCastPerShow(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            // Act & Assert
            var actual = await Assert.ThrowsAsync<Exception>(() => _scraperService.GetAllTvShows(1));

            actual.Message.Should().Be(exception.Message);
            _loggerMock.VerifyLogMessage(LogLevel.Error, expected, Times.Once());
        }
    }
}
