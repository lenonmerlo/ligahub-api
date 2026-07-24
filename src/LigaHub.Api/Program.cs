using LigaHub.Api.ExceptionHandling;
using LigaHub.Application.Organizations;
using LigaHub.Application.Organizations.CreateOrganization;
using LigaHub.Application.Organizations.GetOrganizationById;
using LigaHub.Infrastructure.Persistence;
using LigaHub.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
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
builder.Services.AddScoped<GetOrganizationByIdUseCase>();

var app = builder.Build();
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "LigaHub API v1");
    });
}

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
