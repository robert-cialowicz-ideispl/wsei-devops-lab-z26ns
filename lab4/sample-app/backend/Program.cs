using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// W wersji startowej brak oczekiwania na gotowość SQL Server jest celowy.
// Jeśli inicjalizacja się nie uda, proces zakończy się, a Docker go wznowi.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/api/products", async (AppDbContext db) =>
    Results.Ok(await db.Products.OrderByDescending(p => p.Id).ToListAsync()));

app.MapPost("/api/products", async (Product product, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(product.Name) || product.Name.Length > 200)
    {
        return Results.BadRequest(new { error = "Nazwa produktu musi mieć od 1 do 200 znaków." });
    }

    // Klient podaje wyłącznie nazwę; identyfikator generuje baza.
    var newProduct = new Product { Name = product.Name.Trim(), CreatedAt = DateTime.UtcNow };
    db.Products.Add(newProduct);
    await db.SaveChangesAsync();
    return Results.Created($"/api/products/{newProduct.Id}", newProduct);
});

app.MapGet("/health", () => Results.Ok(new { status = "running" }));
app.Run();

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
