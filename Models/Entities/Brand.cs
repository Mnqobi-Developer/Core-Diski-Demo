using System.ComponentModel.DataAnnotations;

namespace Core_Diski_Demo.Models.Entities;

public class Brand
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
