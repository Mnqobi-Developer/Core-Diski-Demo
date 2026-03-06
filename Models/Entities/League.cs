using System.ComponentModel.DataAnnotations;

namespace Core_Diski_Demo.Models.Entities;

public class League
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(400)]
    public string? Description { get; set; }

    public ICollection<Club> Clubs { get; set; } = new List<Club>();
}
