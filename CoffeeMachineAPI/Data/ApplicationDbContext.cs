using CoffeeMachineAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace CoffeeMachineAPI.Data;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;

    }

    public DbSet<CupSize> CupSizes { get; set; }
    public DbSet<CupStock> CupStockes { get; set; }
    public DbSet<Drink> Drinks { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Machine>  Machines { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CupSize>().HasData(
            new CupSize { Id = 1, Name = "Small", Code = 'S', VolumeInMl = 250, Multiplier = 1.00m},
            new CupSize { Id = 2, Name = "Medium", Code = 'M', VolumeInMl = 400, Multiplier = 1.50m },
            new CupSize { Id = 3, Name = "Large", Code = 'L', VolumeInMl = 500, Multiplier = 2.00m }
        );
        var adminEmail = _configuration["Admin:email"];
        var adminPassword = _configuration["Admin:password"];

        var adminUser = new User
        {
            Id = 1,
            Email = adminEmail,
            IsAdmin = true,
            CreatedAt = DateTime.Now
        };
        adminUser.SetPassword(adminPassword);

        modelBuilder.Entity<User>().HasData(adminUser);
        
        modelBuilder.Entity<Drink>().HasData(
            new Drink { Id = 1, Name = "Espresso", Price = 2.50m, Description = "Rich and bold espresso shot", ImageUrl = "https://example.com/images/espresso.jpg" },
            new Drink { Id = 2, Name = "Latte", Price = 3.50m, Description = "Smooth espresso with steamed milk", ImageUrl = "https://example.com/images/latte.jpg" },
            new Drink { Id = 3, Name = "Cappuccino", Price = 3.75m, Description = "Espresso with steamed milk and foam", ImageUrl = "https://example.com/images/cappuccino.jpg" },
            new Drink { Id = 4, Name = "Americano", Price = 2.75m, Description = "Espresso diluted with hot water", ImageUrl = "https://example.com/images/americano.jpg" },
            new Drink { Id = 5, Name = "Mocha", Price = 4.00m, Description = "Espresso with chocolate and steamed milk", ImageUrl = "https://example.com/images/mocha.jpg" },
            new Drink { Id = 6, Name = "Macchiato", Price = 3.25m, Description = "Espresso with a touch of foam", ImageUrl = "https://example.com/images/macchiato.jpg" },
            new Drink { Id = 7, Name = "Flat White", Price = 3.60m, Description = "Velvety smooth espresso with milk", ImageUrl = "https://example.com/images/flatwhite.jpg" },
            new Drink { Id = 8, Name = "Iced Coffee", Price = 3.00m, Description = "Refreshing cold brewed coffee", ImageUrl = "https://example.com/images/icedcoffee.jpg" },
            new Drink { Id = 9, Name = "Hot Chocolate", Price = 3.20m, Description = "Creamy and rich chocolate drink", ImageUrl = "https://example.com/images/hotchocolate.jpg" },
            new Drink { Id = 10, Name = "Chai Latte", Price = 3.80m, Description = "Spiced tea with steamed milk", ImageUrl = "https://example.com/images/chailatte.jpg" }
        );
       
    }

    
}