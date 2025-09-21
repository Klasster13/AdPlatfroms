using AdPlatforms.Services;
using Microsoft.AspNetCore.Mvc;


namespace AdPlatforms.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AdsController(IDataService dataService) : ControllerBase
{
    private readonly IDataService _dataService = dataService;


    /// <summary>
    /// Upload data from file
    /// </summary>
    /// <param name="file">Data file</param>
    /// <returns>Status code</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/ads
    ///     {
    ///         // IFormFile
    ///     }
    /// </remarks>
    /// <responce code="200">Success</responce>
    /// <responce code="400">Invalid data</responce>
    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<string> Upload(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("File data is invalid.");
        }

        try
        {
            _dataService.UploadPlatformsFromFile(file);
            return "Successful uploading data.";
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
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/ads/search?location=/ru
    /// </remarks>
    /// <responce code="200">Success</responce>
    /// <responce code="500">Error searching</responce>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
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
            return platformsList;
        }
        catch (Exception ex)
        {
            return BadRequest($"Error getting data: {ex.Message}");
        }
    }
}
