using AdsPlatform.Domain.Models;

namespace AdsPlatform.Data;

public interface IDataRepository
{
    void Upload(List<Platform> platforms);
    List<string> Find(string location);
}
