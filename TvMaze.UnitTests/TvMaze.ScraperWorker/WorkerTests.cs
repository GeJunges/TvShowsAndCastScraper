using TvMazeScraper.Service;
using TvMazeScraper.Worker;

namespace TvMaze.UnitTests.TvMaze.ScraperWorker
{
    public class WorkerTests
    {
        private readonly Mock<ILogger<Worker>> _loggerMock;
        private readonly Mock<IScraperService> _scraperServiceMock;
        private readonly Worker _worker;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Fixture _fixture = new();

        public WorkerTests()
        {
            _fixture.Customize(new AutoFixtureCustomization());

            _loggerMock = _fixture.Freeze<Mock<ILogger<Worker>>>();
            _scraperServiceMock = _fixture.Freeze<Mock<IScraperService>>();
            _cancellationTokenSource = new CancellationTokenSource();
            _worker = new Worker(_loggerMock.Object, _scraperServiceMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCallsGetStartingPageOnce()
        {
            // Arrange
            _scraperServiceMock.Setup(s => s.GetStartingPage()).ReturnsAsync(1);

            // Cancel after a short delay to simulate work and then cancel
            _cancellationTokenSource.CancelAfter(2000);

            // Act
            var executeTask = _worker.StartAsync(_cancellationTokenSource.Token);

            // Wait for the worker to complete
            await executeTask;

            // Assert
            _scraperServiceMock.Verify(s => s.GetStartingPage(), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCallsGetAllTvShowsOnEachLoopIteration()
        {
            // Arrange
            int startPage = 1;
            _scraperServiceMock.Setup(s => s.GetStartingPage()).ReturnsAsync(startPage);

            // Cancel after a short delay to simulate work and then cancel
            _cancellationTokenSource.CancelAfter(2000);

            // Act
            var executeTask = _worker.StartAsync(_cancellationTokenSource.Token);

            // Wait for the worker to complete
            await executeTask;

            // Assert
            _scraperServiceMock.Verify(s => s.GetAllTvShows(1, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }
    }
}
