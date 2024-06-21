using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Serilog;
using Workspace.API;
using Workspace.Application;
using Workspace.Domain;
using Workspace.Persistence;
using Workspace.Infrastructure;
using Microsoft.AspNetCore.CookiePolicy;
using Workspace.Contract;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging
    .ClearProviders()
    .AddSerilog();

builder.Host.UseSerilog();

// Add configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});
builder.Services.AddSignalR();


builder.Services.AddConfigureMediatR();
builder.Services.ConfigurePostgreSQLRetryOptions(builder.Configuration.GetSection(nameof(PostgreSQLRetryOptions)));
builder.Services.AddSqlConfiguration();

builder.Services.AddConfigureAutoMapper();
builder.Services.AddRepositoryBaseConfiguration();

builder.Services.AddJWTConfiguration(builder.Configuration);

// Add API

builder.Services.AddControllers().AddApplicationPart(Workspace.Presentation.AssemblyReference.Assembly);

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services
        .AddSwaggerGenNewtonsoftSupport()
        .AddFluentValidationRulesToSwagger()
        .AddEndpointsApiExplorer()
        .AddSwagger();

builder.Services
    .AddApiVersioning(options => options.ReportApiVersions = true)
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddAuthorization();

builder.Services.AddCookiePolicy(options =>
{
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

//app.MapIdentityApi<User>();

// Configure the HTTP request pipeline.
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCookiePolicy();
app.UseStaticFiles();

// Add CORS policy
app.UseCors("CorsPolicy");

//app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/notificationHub");
    endpoints.MapControllers();
});


if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
    app.ConfigureSwagger();

try
{
    await app.RunAsync();
    Log.Information("Stopped cleanly");
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
    await app.StopAsync();
}
finally
{
    Log.CloseAndFlush();
    await app.DisposeAsync();
}
