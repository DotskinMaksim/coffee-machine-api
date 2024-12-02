using CoffeeMachineAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace CoffeeMachineAPI.Data;

public class ApplicationDbContext : DbContext
{
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
            new CupSize { Id = 1, Name = "Small", Code = 'S', VolumeInMl = 250 },
            new CupSize { Id = 2, Name = "Medium", Code = 'M', VolumeInMl = 400 },
            new CupSize { Id = 3, Name = "Large", Code = 'L', VolumeInMl = 500 }
        );
       
    }

    
}