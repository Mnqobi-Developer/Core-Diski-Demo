using Core_Diski_Demo.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core_Diski_Demo.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<League> Leagues => Set<League>();
    public DbSet<Club> Clubs => Set<Club>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Size> Sizes => Set<Size>();
    public DbSet<ProductSize> ProductSizes => Set<ProductSize>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ProductSize>()
            .HasKey(ps => new { ps.ProductId, ps.SizeId });

        builder.Entity<Wishlist>()
            .HasIndex(w => new { w.UserId, w.ProductId })
            .IsUnique();

        builder.Entity<League>().HasData(
            new League { Id = 1, Name = "English Premier League" },
            new League { Id = 2, Name = "Spanish La Liga" },
            new League { Id = 3, Name = "German Bundesliga" },
            new League { Id = 4, Name = "French Ligue 1" },
            new League { Id = 5, Name = "Italian Serie A" },
            new League { Id = 6, Name = "Dutch Eredivisie" },
            new League { Id = 7, Name = "Portuguese Primeira Liga" },
            new League { Id = 8, Name = "South African Betway Premiership" }
        );

        builder.Entity<Brand>().HasData(
            new Brand { Id = 1, Name = "Adidas" },
            new Brand { Id = 2, Name = "Nike" },
            new Brand { Id = 3, Name = "Puma" },
            new Brand { Id = 4, Name = "New Balance" },
            new Brand { Id = 5, Name = "Umbro" },
            new Brand { Id = 6, Name = "Kappa" },
            new Brand { Id = 7, Name = "Macron" },
            new Brand { Id = 8, Name = "Hummel" },
            new Brand { Id = 9, Name = "Joma" },
            new Brand { Id = 10, Name = "Mizuno" },
            new Brand { Id = 11, Name = "Castore" },
            new Brand { Id = 12, Name = "Errea" },
            new Brand { Id = 13, Name = "Under Armour" },
            new Brand { Id = 14, Name = "Kelme" },
            new Brand { Id = 15, Name = "Le Coq Sportif" },
            new Brand { Id = 16, Name = "Reebok" },
            new Brand { Id = 17, Name = "Uhlsport" },
            new Brand { Id = 18, Name = "Diadora" },
            new Brand { Id = 19, Name = "Craft" },
            new Brand { Id = 20, Name = "Lotto" }
        );

        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Home Shirt" },
            new Category { Id = 2, Name = "Away Shirt" },
            new Category { Id = 3, Name = "Third Kit" },
            new Category { Id = 4, Name = "Classic Shirt" },
            new Category { Id = 5, Name = "Retro Shirt" },
            new Category { Id = 6, Name = "Best Seller" },
            new Category { Id = 7, Name = "Training T-Shirt" }
        );



        builder.Entity<Size>().HasData(
            new Size { Id = 1, Name = "Kids" },
            new Size { Id = 2, Name = "Extra Small" },
            new Size { Id = 3, Name = "Small" },
            new Size { Id = 4, Name = "Medium" },
            new Size { Id = 5, Name = "Large" },
            new Size { Id = 6, Name = "Extra Large" }
        );

        builder.Entity<Club>().HasData(
            new Club { Id = 1, Name = "Manchester United", LeagueId = 1, Country = "England" },
            new Club { Id = 2, Name = "Arsenal", LeagueId = 1, Country = "England" },
            new Club { Id = 3, Name = "Liverpool", LeagueId = 1, Country = "England" },
            new Club { Id = 4, Name = "Chelsea", LeagueId = 1, Country = "England" },
            new Club { Id = 5, Name = "Manchester City", LeagueId = 1, Country = "England" },

            new Club { Id = 6, Name = "Real Madrid", LeagueId = 2, Country = "Spain" },
            new Club { Id = 7, Name = "FC Barcelona", LeagueId = 2, Country = "Spain" },
            new Club { Id = 8, Name = "Atletico Madrid", LeagueId = 2, Country = "Spain" },
            new Club { Id = 9, Name = "Sevilla", LeagueId = 2, Country = "Spain" },

            new Club { Id = 10, Name = "Bayern Munich", LeagueId = 3, Country = "Germany" },
            new Club { Id = 11, Name = "Borussia Dortmund", LeagueId = 3, Country = "Germany" },
            new Club { Id = 12, Name = "RB Leipzig", LeagueId = 3, Country = "Germany" },
            new Club { Id = 13, Name = "Bayer Leverkusen", LeagueId = 3, Country = "Germany" },

            new Club { Id = 14, Name = "PSG", LeagueId = 4, Country = "France" },
            new Club { Id = 15, Name = "Olympique Marseille", LeagueId = 4, Country = "France" },
            new Club { Id = 16, Name = "AS Monaco", LeagueId = 4, Country = "France" },
            new Club { Id = 17, Name = "Olympique Lyon", LeagueId = 4, Country = "France" },

            new Club { Id = 18, Name = "Juventus", LeagueId = 5, Country = "Italy" },
            new Club { Id = 19, Name = "AC Milan", LeagueId = 5, Country = "Italy" },
            new Club { Id = 20, Name = "Inter Milan", LeagueId = 5, Country = "Italy" },
            new Club { Id = 21, Name = "Napoli", LeagueId = 5, Country = "Italy" },

            new Club { Id = 22, Name = "Ajax", LeagueId = 6, Country = "Netherlands" },
            new Club { Id = 23, Name = "PSV Eindhoven", LeagueId = 6, Country = "Netherlands" },
            new Club { Id = 24, Name = "Feyenoord", LeagueId = 6, Country = "Netherlands" },

            new Club { Id = 25, Name = "Benfica", LeagueId = 7, Country = "Portugal" },
            new Club { Id = 26, Name = "FC Porto", LeagueId = 7, Country = "Portugal" },
            new Club { Id = 27, Name = "Sporting CP", LeagueId = 7, Country = "Portugal" },

            new Club { Id = 28, Name = "Mamelodi Sundowns", LeagueId = 8, Country = "South Africa" },
            new Club { Id = 29, Name = "Kaizer Chiefs", LeagueId = 8, Country = "South Africa" },
            new Club { Id = 30, Name = "Orlando Pirates", LeagueId = 8, Country = "South Africa" },
            new Club { Id = 31, Name = "SuperSport United", LeagueId = 8, Country = "South Africa" },
            new Club { Id = 32, Name = "Stellenbosch FC", LeagueId = 8, Country = "South Africa" },
            new Club { Id = 33, Name = "AmaZulu FC", LeagueId = 8, Country = "South Africa" }
        );
    }
}
