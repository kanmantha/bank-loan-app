using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add SpaServices for React
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "build";
});

// Configure CORS to allow API calls
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

// Serve static files from wwwroot if needed
app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseRouting();

app.MapControllers();

// Configure SPA
app.UseSpa(spa =>
{
    spa.Options.SourcePath = ".";

    if (app.Environment.IsDevelopment())
    {
        // This will run "npm start" from the project directory
        spa.UseReactDevelopmentServer(npmScript: "start");
    }
});

app.Run();
