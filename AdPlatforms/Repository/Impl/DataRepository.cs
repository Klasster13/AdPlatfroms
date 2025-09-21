using AdPlatforms.Models;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace AdPlatforms.Repository.Impl;

public class DataRepository(ILogger<IDataRepository> logger) : IDataRepository
{
    private readonly ILogger<IDataRepository> _logger = logger;
    private LocationTreeNode _root = new();
    private readonly ConcurrentDictionary<string, List<string>> _cache = [];


    public void UploadData(List<Platform> platforms)
    {
        ArgumentNullException.ThrowIfNull(platforms, "Invalid data provided.");

        var newRoot = new LocationTreeNode();

        _logger.LogDebug("Starting creating data trie. Platforms count: {platformCount}.",
            platforms.Count);
        var stopwatch = Stopwatch.StartNew();

        foreach (var platform in platforms)
        {
            foreach (var location in platform.Locations)
            {
                AddPlatformToTrie(newRoot, location, platform.Name);
            }
        }
        stopwatch.Stop();

        _logger.LogDebug("Finished creating data trie. Elapsed time: {time}ms.",
            stopwatch.ElapsedMilliseconds);

        _root = newRoot;
        var cacheSizeDeleted = _cache.Count;
        _cache.Clear();

        _logger.LogDebug("Cleared cache size: {size}.",
            cacheSizeDeleted);
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
        _logger.LogDebug("Searching for location: {targetLocation}.",
            targetLocation);

        if (string.IsNullOrWhiteSpace(targetLocation))
        {
            _logger.LogWarning("Empty target location provided for search.");
            return [];
        }

        if (_cache.TryGetValue(targetLocation, out var cachedResult))
        {
            _logger.LogDebug("Cache found for location: {targetLocation}.",
                targetLocation);
            return cachedResult;
        }

        _logger.LogDebug("No cache found for location: {targetLocation}.",
                targetLocation);

        var parts = targetLocation.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var currentNode = _root;
        var tmpData = new HashSet<string>();
        var stopwatch = Stopwatch.StartNew();

        foreach (var part in parts)
        {
            if (!currentNode.Children.TryGetValue(part, out currentNode))
            {
                break;
            }
            tmpData.UnionWith(currentNode.PlatformNames);
        }

        var resultData = tmpData.ToList();
        stopwatch.Stop();
        _cache[targetLocation] = resultData;

        _logger.LogDebug("Search coplete. Location: {targetLocation}. Elapsed time {time}ms.",
                targetLocation, stopwatch.ElapsedMilliseconds);

        return resultData;
    }
}
