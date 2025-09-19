using AdsPlatform.Domain.Models;

namespace AdsPlatform.Domain.ParsingService;

public interface IDataService
{
    List<Platform> UploadPlatformsFromFile(IFormFile file);
    List<string> FindPlatromsForLocation(string targetLocation);
}
