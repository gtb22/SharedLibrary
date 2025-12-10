using SimpleMDB.Api.Middleware;
using SimpleMDB.Api.Controllers;
using SimpleMDB.Core;
using SimpleMDB.Api.Services;
using SimpleMDB.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register dependencies
builder.Services.AddSingleton<IMovieRepository, InMemoryMovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Middleware pipeline
app.UseMiddleware<StructuredLoggingMiddleware>();
app.UseMiddleware<CentralizedErrorHandlingMiddleware>();
app.UseMiddleware<UrlQueryStringParsingMiddleware>();
app.UseMiddleware<JsonBodyParsingMiddleware>();
app.UseMiddleware<StaticFileServingMiddleware>();

// Routes
app.MapMoviesRoutes();

app.Run();
