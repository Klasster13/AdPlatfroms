using System.ComponentModel.DataAnnotations;

namespace AdPlatforms.Models;

public class Platform
{
    public string Name { get; set; } = null!;
    public List<string> Locations { get; set; } = null!;
}
