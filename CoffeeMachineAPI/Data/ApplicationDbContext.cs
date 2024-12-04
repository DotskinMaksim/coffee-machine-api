using CoffeeMachineAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeMachineAPI.Data;

// Andmebaasi konteksti klass, mis laiendab DbContext'i
public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configuration;  // Konfiguratsiooni teenus, et lugeda rakenduse seadistusi

    // Konstruktor, mis võtab vastu DbContextOptions ja konfiguratsiooni
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;  // Salvestame konfiguratsiooni teenuse
    }

    // Defineerime DbSet omadused, mis vastavad andmebaasi tabelitele
    public DbSet<CupSize> CupSizes { get; set; }  // Tassi suuruse tabel
    public DbSet<CupStock> CupStockes { get; set; }  // Tassi varude tabel
    public DbSet<Drink> Drinks { get; set; }  // Joogid
    public DbSet<Location> Locations { get; set; }  // Asukohad
    public DbSet<Machine> Machines { get; set; }  // Masinad
    public DbSet<Order> Orders { get; set; }  // Tellimused
    public DbSet<OrderItem> OrderItems { get; set; }  // Tellimuse tooted
    public DbSet<Payment> Payments { get; set; }  // Makse
    public DbSet<User> Users { get; set; }  // Kasutajad
    
    // OnModelCreating meetod, kus määratakse andmebaasi skeem ja algväärtused
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);  // Kutsume esivanema meetodi

        // Täiendame CupSize tabelit algväärtustega
        modelBuilder.Entity<CupSize>().HasData(
            new CupSize { Id = 1, Name = "Small", Code = 'S', VolumeInMl = 250, Multiplier = 1.00m },
            new CupSize { Id = 2, Name = "Medium", Code = 'M', VolumeInMl = 400, Multiplier = 1.50m },
            new CupSize { Id = 3, Name = "Large", Code = 'L', VolumeInMl = 500, Multiplier = 2.00m }
        );

        // Admin kasutaja loomine konfiguratsioonist saadud e-posti ja parooliga
        var adminEmail = _configuration["Admin:email"];
        var adminPassword = _configuration["Admin:password"];

        var adminUser = new User
        {
            Id = 1,
            Email = adminEmail,
            IsAdmin = true,
            CreatedAt = DateTime.Now
        };
        adminUser.SetPassword(adminPassword);  // Parooli seadistamine

        modelBuilder.Entity<User>().HasData(adminUser);  // Lisame admin kasutaja andmebaasi

        // Täiendame Drink tabelit algväärtustega
        modelBuilder.Entity<Drink>().HasData(
            new Drink { Id = 1, Name = "Espresso", Price = 2.50m, Description = "Rich and bold espresso shot", ImageUrl = "https://i.ibb.co/1mmf9rP/latte.jpg" },
            new Drink { Id = 2, Name = "Latte", Price = 3.50m, Description = "Smooth espresso with steamed milk", ImageUrl = "https://i.ibb.co/1mmf9rP/latte.jpg" },
            new Drink { Id = 3, Name = "Cappuccino", Price = 3.75m, Description = "Espresso with steamed milk and foam", ImageUrl = "https://i.ibb.co/25jTDpH/cappuccino.jpg" },
            new Drink { Id = 4, Name = "Americano", Price = 2.75m, Description = "Espresso diluted with hot water", ImageUrl = "https://i.ibb.co/PGBTsKN/americano.jpg" },
            new Drink { Id = 5, Name = "Mocha", Price = 4.00m, Description = "Espresso with chocolate and steamed milk", ImageUrl = "https://i.ibb.co/6RJ2hgy/mocha.jpg" },
            new Drink { Id = 6, Name = "Macchiato", Price = 3.25m, Description = "Espresso with a touch of foam", ImageUrl = "https://i.ibb.co/YZcCz6X/macchiato.jpg" },
            new Drink { Id = 7, Name = "Flat White", Price = 3.60m, Description = "Velvety smooth espresso with milk", ImageUrl = "https://i.ibb.co/DpqZKrT/flatwhite.jpg" },
            new Drink { Id = 8, Name = "Iced Coffee", Price = 3.00m, Description = "Refreshing cold brewed coffee", ImageUrl = "https://i.ibb.co/Mk1WVpC/icedcoffee.jpg" },
            new Drink { Id = 9, Name = "Hot Chocolate", Price = 3.20m, Description = "Creamy and rich chocolate drink", ImageUrl = "https://i.ibb.co/VwSxHm4/hotchocolate.jpg" },
            new Drink { Id = 10, Name = "Chai Latte", Price = 3.80m, Description = "Spiced tea with steamed milk", ImageUrl = "https://i.ibb.co/hyG4VCZ/chailatte.jpg" }
        );
    }
}