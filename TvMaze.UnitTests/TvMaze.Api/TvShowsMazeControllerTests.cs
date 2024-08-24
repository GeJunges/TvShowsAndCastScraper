using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TvMazeScraper.Controllers;
using TvMazeScraper.Service;
using TvMazeScraper.Service.Models;
using TvMazeScraper.UnitTests;

namespace TvMaze.UnitTests.TvMazeScraper.Api
{
    public class TvShowsMazeControllerTests
    {
        private readonly TvShowsMazeController _controller;
        private readonly Mock<ILogger<TvShowsMazeController>> _loggerMock;
        private readonly Mock<ITvShowService> _tvShowServiceMock;
        private readonly Fixture _fixture = new();

        public TvShowsMazeControllerTests()
        {
            _fixture.Customize(new AutoFixtureCustomization());
            _loggerMock = _fixture.Freeze<Mock<ILogger<TvShowsMazeController>>>();
            _tvShowServiceMock = _fixture.Freeze<Mock<ITvShowService>>();
            _controller = new TvShowsMazeController(_loggerMock.Object, _tvShowServiceMock.Object);
        }

        [Fact]
        public async Task Get_ShouldReturnBadRequest_WhenPageIsInvalid()
        {
            // Arrange
            int invalidPage = 0;
            var expected = $"Page number {invalidPage} is invalid.";

            // Act
            var result = await _controller.Get(invalidPage);

            // Assert
            var actual = Assert.IsType<BadRequestObjectResult>(result.Result);
            actual.Value.Should().Be(expected);
            _loggerMock.VerifyLogMessage(LogLevel.Error, expected, Times.Once());
        }

        [Fact]
        public async Task Get_ShouldReturnOk_WhenValidRequest()
        {
            // Arrange
            int page = 1;
            int itemsPerPage = 25;
            var expectedTvShows = _fixture.Create<List<TvShowResponse>>();

            _tvShowServiceMock.Setup(service => service.GetAllTvShows(page, itemsPerPage, default))
                              .ReturnsAsync(expectedTvShows);

            // Act
            var result = await _controller.Get(page, itemsPerPage);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actual = Assert.IsType<List<TvShowResponse>>(okResult.Value);

            actual.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Get_ShouldCallService_WithCorrectParameters()
        {
            // Arrange
            int page = 1;
            int itemsPerPage = 25;
            var expectedTvShows = _fixture.Create<List<TvShowResponse>>();

            _tvShowServiceMock.Setup(service => service.GetAllTvShows(page, itemsPerPage, default))
                              .ReturnsAsync(expectedTvShows);

            // Act
            var result = await _controller.Get(page, itemsPerPage);

            // Assert
            _tvShowServiceMock.Verify(mock => mock.GetAllTvShows(page, itemsPerPage, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task Get_ShouldReturnInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            int page = 1;
            int itemsPerPage = 25;
            var expected = "Service error";

            _tvShowServiceMock.Setup(service => service.GetAllTvShows(page, itemsPerPage, default))
                              .ThrowsAsync(new Exception(expected));

            // Act
            var result = await _controller.Get(page, itemsPerPage);

            // Assert
            var actual = Assert.IsType<ObjectResult>(result.Result);
            actual.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            actual.Value.Should().Be(expected);
        }

        [Fact]
        public async Task Get_ShouldLog_WhenExceptionThrown()
        {
            // Arrange
            int page = 1;
            int itemsPerPage = 25;
            var expected = $"Something went wrong to fetch page number: {page}";

            _tvShowServiceMock.Setup(service => service.GetAllTvShows(page, itemsPerPage, default))
                              .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.Get(page, itemsPerPage);

            // Assert
            Assert.IsType<ObjectResult>(result.Result);

            _loggerMock.VerifyLogMessage(LogLevel.Error, expected, Times.Once());
        }
    }
}
