using AdPlatforms.Models;
using AdPlatforms.Repository;
using System.Diagnostics;

namespace AdPlatforms.Services.Impl;

public class DataService(IDataRepository dataRepository, ILogger<IDataService> logger) : IDataService
{
    private readonly IDataRepository _dataRepository = dataRepository;
    private readonly ILogger<IDataService> _logger = logger;


    public void UploadPlatformsFromFile(IFormFile file)
    {
        _logger.LogDebug("Starting file upload processing. File: {fileName}, Size: {fileSize} bytes",
            file.FileName, file.Length);

        var stopwatch = Stopwatch.StartNew();
        var data = ParseFile(file);
        stopwatch.Stop();

        _logger.LogDebug("File parsing successful in {time}ms.",
            stopwatch.ElapsedMilliseconds);

        _dataRepository.UploadData(data);
    }


    public List<string> FindPlatromsForLocation(string targetLocation)
    {
        _logger.LogDebug("Starting search process for location: {targetLocation}",
            targetLocation);

        return _dataRepository.FindPlatromsForLocation(targetLocation);
    }


    /// <summary>
    /// Parsing file
    /// </summary>
    /// <param name="file">File from request</param>
    /// <returns>List of platforms from file</returns>
    private List<Platform> ParseFile(IFormFile file)
    {
        _logger.LogDebug("Parsing file: {fileName}", file.FileName);

        if (file is null || file.Length == 0)
        {
            return [];
        }

        var platforms = new List<Platform>();
        var lineCount = 0;

        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            string? line;

            while ((line = reader.ReadLine()) is not null)
            {
                lineCount++;

                if (string.IsNullOrWhiteSpace(line))
                {
                    _logger.LogWarning("Skipping empty line {lineNumber}", lineCount);
                    continue;
                }

                var parts = line.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                {
                    _logger.LogWarning("Invalid format in line {lineNumber}: '{lineContent}'. Expected 'Name:Location1,Location2'",
                        lineCount, line);
                    continue;
                }

                var platformName = parts[0].Trim();
                var locations = parts[1]
                    .Split(",")
                    .Select(l => l.Trim())
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .ToList();

                if (locations.Count == 0)
                {
                    _logger.LogWarning("No valid locations found for platform '{platformName}' in line {lineNumber}",
                        platformName, lineCount);
                    continue;
                }

                var platform = new Platform
                {
                    Name = platformName,
                    Locations = locations
                };

                platforms.Add(platform);
            }
        }

        _logger.LogDebug("File parsing completed. Lines: {totalLines}, Platforms: {platformCount}",
                lineCount, platforms.Count);

        return platforms;
    }
}
