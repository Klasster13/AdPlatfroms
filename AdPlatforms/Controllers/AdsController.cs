using AdPlatforms.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AdPlatforms.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdsController(IDataService dataService, ILogger<AdsController> logger) : ControllerBase
{
    private readonly IDataService _dataService = dataService;
    private readonly ILogger<AdsController> _logger = logger;


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
        _logger.LogInformation("Upload file request received. File name: {fileName}, Size: {fileSize}",
            file?.FileName, file?.Length);

        if (file is null || file.Length == 0)
        {
            _logger.LogWarning("Upload failed: File is null or empty. File: {fileName}, Size: {fileSize}",
                file?.FileName, file?.Length);

            return BadRequest("File is null or empty.");
        }

        try
        {
            _dataService.UploadPlatformsFromFile(file);

            _logger.LogInformation("Successful uploading file: {fileName}, Size: {fileSize}",
                file.FileName, file.Length);

            return "Successful uploading file.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {fileName}, Size: {fileSize} bytes. Error: {message}",
                file.FileName, file.Length, ex.Message);

            return StatusCode(500, $"Error loading file: {ex.Message}");
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
        _logger.LogInformation("Search platforms request received. Location: {location}",
            location);

        if (string.IsNullOrWhiteSpace(location))
        {
            _logger.LogWarning("Search failed: Valid location parameter is required.");

            return BadRequest("Valid location parameter is required.");
        }

        try
        {
            var platformsList = _dataService.FindPlatromsForLocation(location);

            _logger.LogInformation("Successful search. Location: {location}, Found {count} platforms",
                location, platformsList.Count);

            return platformsList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting data for location: {location}. Error: {message}",
                location, ex.Message);

            return StatusCode(500, $"Error getting data: {ex.Message}");
        }
    }
}
