using IcMarkets.Infrastructure;
using IcMarkets.UseCases;
using IcMarkets.WebApi.HealthChecks;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var corsPolicy = "_corsPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy,
        policy =>
        {
            policy.WithOrigins("https://ya.ru", "https://www.google.com")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<IcMarketsDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.ConfigureUseCases();

var baseAddress = builder.Configuration[$"{Constants.BlockCypher}:BaseAddress"]!;
builder.Services.AddHttpClient(Constants.BlockCypher, client =>
{
    client.BaseAddress = new Uri(baseAddress);
});

builder.Services.AddHealthChecks()
    .AddCheck<ExternalApiHealthCheck>("ExternalApiHealthCheck")
    .AddDbContextCheck<IcMarketsDbContext>(name: "ef-core-db", tags: ["database"]);

var app = builder.Build();

app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(corsPolicy);

// app.UseAuthorization();

app.MapControllers();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.Run();
