namespace AdPlatforms.Models;

public class LocationTreeNode
{
    public Dictionary<string, LocationTreeNode> Children { get; } = [];
    public HashSet<string> PlatformNames { get; } = [];
}
