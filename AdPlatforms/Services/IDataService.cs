namespace AdPlatforms.Services;

public interface IDataService
{
    /// <summary>
    /// Upload data to repository
    /// </summary>
    /// <param name="file">File from request</param>
    void UploadPlatformsFromFile(IFormFile file);



    /// <summary>
    /// Gets list of platforms for target location
    /// </summary>
    /// <param name="targetLocation">Target location for searching</param>
    /// <returns>List of platforms</returns>
    List<string> FindPlatromsForLocation(string targetLocation);
}
