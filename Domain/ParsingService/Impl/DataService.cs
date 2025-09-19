
using AdsPlatform.Domain.Models;

namespace AdsPlatform.Domain.ParsingService.Impl;

public class DataService : IDataService
{
    /// <summary>
    /// Parsing file
    /// </summary>
    /// <param name="file">File from request</param>
    /// <returns>List of platforms from file</returns>
    public List<Platform> UploadPlatformsFromFile(IFormFile file)
    {
        var platforms = new List<Platform>();

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

                var parts = line.Split(':', 2);

                // Skipping invalid lines
                if (parts.Length != 2)
                {
                    continue;
                }

                var platform = new Platform
                {
                    Name = parts[0].Trim(),
                    Locations = [.. parts[1]
                    .Split(",")
                    .Select(l => l.Trim())
                    .Where(l => !string.IsNullOrWhiteSpace(l))]
                };

            }
        }

        return platforms;
    }



    public List<string> FindPlatromsForLocation(string targetLocation)
    {
        throw new NotImplementedException();
    }
}
