using AdPlatforms.Models;
using AdPlatforms.Repository;

namespace AdPlatforms.Services.Impl;

public class DataService(IDataRepository dataRepository) : IDataService
{
    private readonly IDataRepository _dataRepository = dataRepository;


    public void UploadPlatformsFromFile(IFormFile file)
    {
        var data = ParseFile(file);
        _dataRepository.UploadData(data);
    }


    public List<string> FindPlatromsForLocation(string targetLocation)
    {
        return _dataRepository.FindPlatromsForLocation(targetLocation);
    }


    /// <summary>
    /// Parsing file
    /// </summary>
    /// <param name="file">File from request</param>
    /// <returns>List of platforms from file</returns>
    private static List<Platform> ParseFile(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return [];
        }

        var platforms = new List<Platform>();

        // Opening data reading stream from file
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            string? line;

            // Parsing file till end
            while ((line = reader.ReadLine()) is not null)
            {
                // Skipping empty lines
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var parts = line.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);

                // Skipping invalid lines
                if (parts.Length != 2)
                {
                    continue;
                }

                // Creating new platform entity
                var platform = new Platform
                {
                    Name = parts[0].Trim(),
                    Locations = [.. parts[1]
                    .Split(",")
                    .Select(l => l.Trim())
                    .Where(l => !string.IsNullOrWhiteSpace(l))]
                };

                platforms.Add(platform);
            }
        }

        return platforms;
    }
}
