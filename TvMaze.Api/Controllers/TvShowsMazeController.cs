using Microsoft.AspNetCore.Mvc;
using TvMazeScraper.Service;
using TvMazeScraper.Service.Models;

namespace TvMazeScraper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TvShowsMazeController : ControllerBase
    {
        private readonly ILogger<TvShowsMazeController> _logger;
        private readonly ITvShowService _tvShowService;

        public TvShowsMazeController(ILogger<TvShowsMazeController> logger, ITvShowService tvShowService)
        {
            _logger = logger;
            _tvShowService = tvShowService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TvShowResponse>>> Get([FromQuery] int page, [FromQuery] int itensPerPage = 25)
        {
            try
            {
                if (page <= 0)
                {
                    var message = $"Page number {page} is invalid.";
                    _logger.LogError(message);
                    return BadRequest(message);
                }

                var response = await _tvShowService.GetAllTvShows(page, itensPerPage);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong to fetch page number: {page}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
