using CoffeeMachineAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CoffeeMachineAPI.Data
{
    // Rakenduse andmebaasi konteksti klass, mis laiendab DbContext'i
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        // Konstruktor, mis saab konfiguratsiooni ja DbContextOptions objektid
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration; // Konfiguratsiooniga töötamiseks salvestame selle
        }

        // Peamised tabelid, mis on seotud andmebaasis olevate mudelitega
        public DbSet<CupSize> CupSizes { get; set; }
        public DbSet<CupStock> CupStockes { get; set; }
        public DbSet<Drink> Drinks { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<User> Users { get; set; }

        // Auditiandmete tabel
        public DbSet<AuditLog> AuditLogs { get; set; }

        // Logi tabel logimise jaoks
        public DbSet<LoginLog> LoginLogs { get; set; }

        // Mudeli loomine andmebaasi jaoks
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Põhiloogika kutsumine

            // Algandmete lisamine tabelitesse
            modelBuilder.Entity<CupSize>().HasData(
                new CupSize { Id = 1, Name = "Small", Code = 'S', VolumeInMl = 250, Multiplier = 1.00m },
                new CupSize { Id = 2, Name = "Medium", Code = 'M', VolumeInMl = 400, Multiplier = 1.50m },
                new CupSize { Id = 3, Name = "Large", Code = 'L', VolumeInMl = 500, Multiplier = 2.00m }
            );

            // Administraatori lisamine süsteemi konfiguratsiooni kaudu
            var adminEmail = _configuration["Admin:email"];
            var adminPassword = _configuration["Admin:password"];
            var adminUser = new User
            {
                Id = 1,
                Email = adminEmail,
                IsAdmin = true,
                CreatedAt = DateTime.Now
            };
            adminUser.SetPassword(adminPassword); // Admini parooli määramine
            var unknownUser = new User
            {
                Id = 1000,
                Email = "unknown",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                PasswordHash = string.Empty
            };
            modelBuilder.Entity<User>().HasData(adminUser, unknownUser); // Admini lisamine andmebaasi
            
           


            // Algandmete lisamine jookide tabelisse
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

        // Üksikasjalik muudatuste salvestamine, mis käsitleb auditiandmete salvestamist
        public override int SaveChanges()
        {
            var auditEntries = PrepareAuditEntries(); // Muudatuste ettevalmistamine auditi jaoks
            var result = base.SaveChanges(); // Andmete salvestamine
            FinalizeAuditEntries(auditEntries); // Auditiandmete lõplik salvestamine
            return result;
        }

        // Asünkroonne muudatuste salvestamine, mis käsitleb auditiandmete salvestamist
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = PrepareAuditEntries(); // Muudatuste ettevalmistamine auditi jaoks
            var result = await base.SaveChangesAsync(cancellationToken); // Asünkroonne salvestamine
            await FinalizeAuditEntriesAsync(auditEntries, cancellationToken); // Auditiandmete lõplik salvestamine
            return result;
        }

        // Auditilogide ettevalmistamine muudatuste salvestamiseks
        private List<AuditEntry> PrepareAuditEntries()
        {
            ChangeTracker.DetectChanges(); // Muudatuste tuvastamine
            var auditEntries = new List<AuditEntry>(); // Loome loendi auditiandmete jaoks

            foreach (var entry in ChangeTracker.Entries())
            {
                
                // Vältige auditi logi loomist, kui tegemist on auditi logiga või kui entiteet ei ole muutunud
                if (entry.Entity is AuditLog || entry.Entity is LoginLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;


                // Loome auditi kirje vastavalt muudatuse tüübile (Lisa, Muuda, Kustuta)
                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Entity.GetType().Name, // Entiteedi nimi
                    Action = entry.State switch
                    {
                        EntityState.Added => "Insert", // Lisamine
                        EntityState.Deleted => "Delete", // Kustutamine
                        EntityState.Modified => "Update", // Muutmine
                        _ => "Unknown action" // Teadmata toiming
                    }
                };

                // Peamine võti
                var primaryKey = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
                if (primaryKey != null)
                {
                    auditEntry.EntityId = primaryKey.CurrentValue?.ToString(); // Salvestame peamise võtme väärtuse
                }

                // Kõik omadused
                foreach (var property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        auditEntry.TemporaryProperties.Add(property); // Ajutised omadused
                        continue;
                    }

                    string propertyName = property.Metadata.Name;
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue; // Lisamine
                            break;
                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = property.OriginalValue; // Kustutamine
                            break;
                        case EntityState.Modified when property.IsModified:
                            auditEntry.OldValues[propertyName] = property.OriginalValue; // Muudatuse vanad väärtused
                            auditEntry.NewValues[propertyName] = property.CurrentValue; // Muudatuse uued väärtused
                            break;
                    }
                }

                auditEntries.Add(auditEntry); // Lisame auditi kirje
            }

            return auditEntries; // Tagastame kõik auditi kirjed
        }

        // Auditikirjete salvestamine
        private void FinalizeAuditEntries(List<AuditEntry> auditEntries)
        {
            if (!auditEntries.Any()) return;

            foreach (var auditEntry in auditEntries)
            {
                AuditLogs.Add(auditEntry.ToAuditLog()); // Lisame auditilogisse
            }

            SaveChanges(); // Lõplik salvestamine
        }

        // Asünkroonne auditikirjete salvestamine
        private async Task FinalizeAuditEntriesAsync(List<AuditEntry> auditEntries, CancellationToken cancellationToken)
        {
            if (!auditEntries.Any()) return;

            foreach (var auditEntry in auditEntries)
            {
                AuditLogs.Add(auditEntry.ToAuditLog()); // Lisame auditilogisse
            }

            await SaveChangesAsync(cancellationToken); // Lõplik asünkroonne salvestamine
        }
    }
}