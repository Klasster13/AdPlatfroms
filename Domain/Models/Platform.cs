using System.ComponentModel.DataAnnotations;

namespace AdsPlatform.Domain.Models;

public class Platform
{
    [Required]
    public string Name { get; set; } = null!;


    [Required]
    public List<string> Locations { get; set; } = null!;
}
