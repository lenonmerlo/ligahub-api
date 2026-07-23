using LigaHub.Application.Organizations;
using LigaHub.Application.Organizations.CreateOrganization;
using LigaHub.Infrastructure.Persistence;
using LigaHub.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration
    .GetConnectionString("Database")
    ?? throw new InvalidOperationException(
        "The database connection string is not configured.");

builder.Services.AddDbContext<LigaHubDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<
    IOrganizationRepository,
    OrganizationRepository>();

builder.Services.AddScoped<CreateOrganizationUseCase>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
