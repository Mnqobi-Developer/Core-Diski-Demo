using Core_Diski_Demo.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Core_Diski_Demo.Models.ViewModels.Admin;

public class ProductFormViewModel
{
    public int? Id { get; set; }

    [Required, StringLength(180)]
    public string Name { get; set; } = string.Empty;

    [Required, Range(0.01, 100000)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(20)]
    public string? ReleaseSeason { get; set; }

    [Required]
    public ShirtType ShirtType { get; set; }

    [Display(Name = "Club"), Range(1, int.MaxValue, ErrorMessage = "Please select a club.")]
    public int ClubId { get; set; }

    [Display(Name = "Brand"), Range(1, int.MaxValue, ErrorMessage = "Please select a brand.")]
    public int BrandId { get; set; }

    [Display(Name = "Category"), Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
    public int CategoryId { get; set; }

    public List<SelectListItem> Clubs { get; set; } = new();
    public List<SelectListItem> Brands { get; set; } = new();
    public List<SelectListItem> Categories { get; set; } = new();
}
