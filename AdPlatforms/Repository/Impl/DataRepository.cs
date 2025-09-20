using AdPlatforms.Models;
using System.Collections.Concurrent;

namespace AdPlatforms.Repository.Impl;

public class DataRepository : IDataRepository
{
    private LocationTreeNode _root = new();
    private readonly ConcurrentDictionary<string, List<string>> _cache = [];


    public void UploadData(List<Platform> platforms)
    {
        ArgumentNullException.ThrowIfNull(platforms, "Invalid data provided.");

        var newRoot = new LocationTreeNode();

        foreach (var platform in platforms)
        {
            foreach (var location in platform.Locations)
            {
                AddPlatformToTrie(newRoot, location, platform.Name);
            }
        }

        _root = newRoot;
        _cache.Clear();
    }


    /// <summary>
    /// Add platform to trie
    /// </summary>
    /// <param name="root">Starting node</param>
    /// <param name="location">Location to add</param>
    /// <param name="platformName">Platform name to add</param>
    private static void AddPlatformToTrie(LocationTreeNode root, string location, string platformName)
    {
        var currentNode = root;
        var parts = location.Split('/', StringSplitOptions.RemoveEmptyEntries);

        // Checking every part of location
        foreach (var part in parts)
        {
            // Adding new node if current part of location 
            // is not existing in current node children 
            if (!currentNode.Children.TryGetValue(part, out var value))
            {
                value = new LocationTreeNode();
                currentNode.Children[part] = value;
            }
            currentNode = value;
        }
        // Adding platform name to final location part
        currentNode.PlatformNames.Add(platformName);
    }


    public List<string> FindPlatromsForLocation(string targetLocation)
    {
        if (string.IsNullOrWhiteSpace(targetLocation))
        {
            return [];
        }

        // Trying to find data saved in cache
        if (_cache.TryGetValue(targetLocation, out var cachedResult))
        {
            return cachedResult;
        }

        var parts = targetLocation.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var currentNode = _root;
        var tmpData = new HashSet<string>();

        // Searchin data in trie
        foreach (var part in parts)
        {
            // Checking if current location part is existing in current node children
            if (!currentNode.Children.TryGetValue(part, out currentNode))
            {
                break;
            }
            tmpData.UnionWith(currentNode.PlatformNames);
        }

        var resultData = tmpData.ToList();

        // Caching result
        _cache[targetLocation] = resultData;

        return resultData;
    }
}
