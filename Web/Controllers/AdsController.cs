using AdsPlatform.Domain.ParsingService;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace AdsPlatform.Web.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AdsController(/*ILogger logger,*/ IDataService dataService) : ControllerBase
{
    // TODO LOGGER?
    //private readonly ILogger _logger = logger;
    private readonly IDataService _dataService = dataService;


    /// <summary>
    /// Upload data from file
    /// </summary>
    /// <param name="file">Data file</param>
    /// <returns>Status code</returns>
    /// Sample request:
    /// 
    ///     POST /api/ads
    /// </remarks>
    /// <responce code="200">Success</responce>
    /// <responce code="400">Invalid data</responce>
    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public IActionResult Upload([FromBody] IFormFile file)
    {
        try
        {
            _dataService.UploadPlatformsFromFile(file);
            return Ok("Successful uploading data.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error loading data: {ex.Message}");
        }
    }


    /// <summary>
    /// Search platforms for target location
    /// </summary>
    /// <param name="location">Location for searching</param>
    /// <returns>List of found platforms</returns>
    /// /// Sample request:
    /// 
    ///     GET /api/ads/search?location=/ru
    /// </remarks>
    /// <responce code="200">Success</responce>
    /// <responce code="500">Error searching</responce>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<List<string>> GetPlatforms([FromQuery] string location)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            return BadRequest("Location parameter is required");
        }

        try
        {
            var platformsList = _dataService.FindPlatromsForLocation(location);
            return Ok(platformsList);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
