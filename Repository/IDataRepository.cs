using AdPlatforms.Models;

namespace AdPlatforms.Repository;

public interface IDataRepository
{
    /// <summary>
    /// Upload new data
    /// </summary>
    /// <param name="platforms">List of platform data to upload</param>
    void UploadData(List<Platform> platforms);


    /// <summary>
    /// Gets list of platforms for target location
    /// </summary>
    /// <param name="targetLocation">Target location for searching</param>
    /// <returns>List of platforms</returns>
    List<string> FindPlatromsForLocation(string targetLocation);
}
