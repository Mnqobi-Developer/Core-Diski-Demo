using System.ComponentModel.DataAnnotations;

namespace Core_Diski_Demo.Models.Entities;

public class Size
{
    public int Id { get; set; }

    [Required, StringLength(30)]
    public string Name { get; set; } = string.Empty;

    public ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
}
