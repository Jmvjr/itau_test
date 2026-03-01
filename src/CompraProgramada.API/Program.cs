using CompraProgramada.Infrastructure;
using CompraProgramada.Infrastructure.Jobs;
using CompraProgramada.Application; // Force load Application assembly
using CompraProgramada.Application.Jobs;
using MediatR;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Infrastructure Layer (DbContext + Repositories)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddInfrastructure(connectionString);

// Configure Hangfire
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString, new Hangfire.SqlServer.SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer();

// Register Jobs Service
builder.Services.AddScoped<IComprasProgramadasJobService, HangfireComprasProgramadasJobService>();

// Configure MediatR - Load handlers from Application assembly explicitly
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CompraProgramada.Application.Commands.CriarClienteCommand>());

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Initialize Hangfire jobs on startup
using (var scope = app.Services.CreateScope())
{
    var jobService = scope.ServiceProvider.GetRequiredService<IComprasProgramadasJobService>();
    await jobService.RegistrarJobsRecurrentesAsync();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Compra Programada API v1.0");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHangfireDashboard("/hangfire");
app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.MapControllers();

app.Run();
