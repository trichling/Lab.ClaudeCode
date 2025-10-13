using LogicQuiz.Api.Data;
using LogicQuiz.Api.Services;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add CORS - Allow all origins for development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add PostgreSQL database
builder.AddNpgsqlDbContext<QuizDbContext>("quizdb");

// Add application services
builder.Services.AddScoped<IQuizService, QuizService>();

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseRouting();
app.MapControllers();

// Seed database with retry logic
var retryPipeline = new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 10,
        Delay = TimeSpan.FromSeconds(2),
        BackoffType = DelayBackoffType.Exponential,
        OnRetry = args =>
        {
            app.Logger.LogWarning("Database connection failed. Attempt {Attempt}. Retrying in {Delay}...",
                args.AttemptNumber, args.RetryDelay);
            return ValueTask.CompletedTask;
        }
    })
    .Build();

await retryPipeline.ExecuteAsync(async cancellationToken =>
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<QuizDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("Attempting to connect to database and apply migrations...");

    // Apply migrations
    await dbContext.Database.MigrateAsync(cancellationToken);

    logger.LogInformation("Migrations applied successfully. Seeding data...");

    // Seed data
    await DbSeeder.SeedAsync(dbContext);

    logger.LogInformation("Database initialization completed successfully.");
}, CancellationToken.None);

app.Run();
