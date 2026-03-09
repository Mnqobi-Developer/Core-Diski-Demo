using System.ComponentModel.DataAnnotations;

namespace Core_Diski_Demo.Models.Entities;

public class Club
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [StringLength(120)]
    public string? Country { get; set; }

    public int LeagueId { get; set; }
    public League League { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
