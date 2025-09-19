using AdsPlatform.Domain.Models;
using System.Collections.Concurrent;

namespace AdsPlatform.Data.Impl;

public class DataRepository : IDataRepository
{
    private ConcurrentDictionary<string, HashSet<string>> _data = [];
    private readonly ConcurrentDictionary<string, List<string>> _cache = [];


    public void Upload(List<Platform> platforms)
    {
        var newData = new ConcurrentDictionary<string, HashSet<string>>();

        foreach (var platform in platforms)
        {
            foreach (var location in platform.Locations)
            {
                // TODO
            }
        }


        _data = newData;
        _cache.Clear();
    }


    public List<string> Find(string targetLocation)
    {
        // Trying to find data saved in cache
        if (_cache.TryGetValue(targetLocation, out var cachedResult))
        {
            return cachedResult;
        }


        var intermediateData = new HashSet<string>();


        if (_data.TryGetValue(targetLocation, out var platforms))
        {
            intermediateData.UnionWith(platforms);
        }

        // TODO search


        var resultData = intermediateData.ToList();

        // Caching result
        _cache[targetLocation] = resultData;

        return resultData;
    }
}
