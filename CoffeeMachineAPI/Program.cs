using System.Net.Http.Headers;
using CoffeeMachineAPI.Controllers;
using CoffeeMachineAPI.Data;
using CoffeeMachineAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args); // Rakenduse ehitaja loomine

// Lisa teenused konteinerisse (DI - dependency injection)

builder.Services.AddControllers(); // Lisab kontrollereid (kontrollerid, mis käsitlevad HTTP-päringuid)
// Lisage OpenAPI konfiguratsioon Swaggeri jaoks
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Swaggeri genereerimine

// Lisame andmebaasi ühenduse (kasutame MySQL-i)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"), // Ühenduse string konfigureeritud applicationsettings.json failis
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")) // Automaatne MySQL serveri versiooni määramine
    )
);

// CORS (Cross-Origin Resource Sharing) poliitika konfigureerimine
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", // Poliitika nimeks on "AllowFrontend"
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Lubame päringud ainult määratud päritolu aadressilt (nt React frontend)
                .AllowAnyHeader() // Lubame kõik päised
                .AllowAnyMethod() // Lubame kõik HTTP meetodid (GET, POST, PUT, DELETE jne)
                .AllowCredentials(); // Lubame küpsiste saatmise koos päringutega (väga oluline autentimise puhul)
        });
});

// Lisame HttpClient teenuse (kõikvõimalikud HTTP päringud API-le)
builder.Services.AddHttpClient();

// Lisame teenuse IImageUploadService, mis kasutab ImgbbImageUploadService teenust (piltide üleslaadimine Imgbb teenusesse)
builder.Services.AddTransient<IImageUploadService, ImgbbImageUploadService>();

var app = builder.Build(); // Rakenduse ehitamine

// Konfigureerime HTTP päringute töötluse toru (middleware pipeline)
if (app.Environment.IsDevelopment()) // Kui arendusrežiimis
{
    app.MapOpenApi(); // Lisame OpenAPI toetus (Swagger ja dokumentatsioon)
    app.UseSwagger(); // Luba Swagger teenus
    app.UseSwaggerUI(); // Luba Swagger UI (veebiliides API testimiseks)
}

// Kasutame CORS poliitikat, et lubada päringud määratud päritolust
app.UseCors("AllowFrontend");

// Luba HTTPS-i redirekteerimine (kõik HTTP päringud suunatakse automaatselt HTTPS-ile)
app.UseHttpsRedirection();

// Luba autentimine ja autoriseerimine (kui on määratud)
app.UseAuthorization();

// Kaardistame kontrollerite tegevused (API lõpp-punktid)
app.MapControllers();

// Rakendus käivitub
app.Run();