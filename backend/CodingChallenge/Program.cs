using CodingChallenge.Data;
using CodingChallenge.Data.Repositories;
using CodingChallenge.Interfaces;
using CodingChallenge.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins("http://localhost:4200") // Allow only your Angular app
            .AllowAnyMethod()
            .AllowAnyHeader());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICryptoPriceService, CryptoService>();
builder.Services.AddScoped<ICryptoPriceRepository, CryptoPriceRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    for (int i = 0; i < 5; i++) // Retry up to 5 times
    {
        try
        {
            db.Database.Migrate();
            Console.WriteLine("Database migration applied successfully.");
            break;
        }
        catch (SqlException)
        {
            Console.WriteLine("Database connection failed. Retrying...");
            System.Threading.Thread.Sleep(5000); // Wait 5 seconds before retry
        }
    }
}

// Add WebSocketService only after migration is done
builder.Services.AddHostedService<WebSocketService>();

// Middleware setup
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request URL: {context.Request.Path}");
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();
app.Run();
