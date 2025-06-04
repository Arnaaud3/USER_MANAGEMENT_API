using System.Net.Mime;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddSingleton<UserService>();
builder.Logging.ClearProviders(); // Clear default logging providers

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // In production, use a custom error page or middleware
    app.UseExceptionHandler("/error");
}
// Create a custom middleware to use the API KEY for authentication 
app.Use(async (context, next) =>
{
    // Check for the presence of an API key in the request headers
    var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
    var apiKey = configuration["ApiKey"];
    if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey) || extractedApiKey != apiKey)
    {
        context.Response.StatusCode = 401; // Unauthorized status code
        await context.Response.WriteAsync("Unauthorized : Invalid or missing API key.");
        // stop the request pipeline here
        return;
    }
    await next(); // Call the next middleware in the pipeline if the authentication is sucessful
});
// Configure a custom middleware to log incoming requests informing about HTTP method and path
app.Use(async (context, next) =>
{
    Log.Information("HTTP {Method} request to {Path}", context.Request.Path, context.Request.Path);
    await next();
});

// Apparently, the following line is not needed as it is already included in the AddControllers method
app.UseRouting();
// Configure authentication and authorization to acces the API endpoints
app.UseAuthentication();
app.UseAuthorization();
// Use Serilog request logging middleware instead of the custom one if necessary 
// app.UseSerilogRequestLogging();
app.MapControllers();
app.Run();
