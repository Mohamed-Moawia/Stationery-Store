using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Serilog;
using StationeryStore.API.Extensions;
using StationeryStore.Infrastructure.Data;
using StationeryStore.Infrastructure.Data.Seeders;
using Npgsql.EntityFrameworkCore.PostgreSQL.NodaTime; // enables UseNodaTime extension
using Microsoft.Extensions.Caching.StackExchangeRedis; // AddStackExchangeRedisCache extension

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for Egyptian logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Egyptian date format
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
    });

// Configure Egyptian culture
var egyptianCulture = new CultureInfo("ar-EG")
{
    NumberFormat =
    {
        CurrencySymbol = "ج.م",
        CurrencyDecimalDigits = 2,
        CurrencyDecimalSeparator = ".",
        CurrencyGroupSeparator = ",",
        NumberDecimalDigits = 3, // For piasters
        NumberDecimalSeparator = ".",
        NumberGroupSeparator = ","
    },
    DateTimeFormat =
    {
        ShortDatePattern = "dd/MM/yyyy",
        LongDatePattern = "dd MMMM yyyy",
        ShortTimePattern = "hh:mm tt",
        LongTimePattern = "hh:mm:ss tt",
        Calendar = new GregorianCalendar()
    }
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        egyptianCulture,
        new CultureInfo("en-US")
    };

    options.DefaultRequestCulture = new RequestCulture("ar-EG");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
    options.RequestCultureProviders.Insert(1, new CookieRequestCultureProvider());
});

// Configure PostgreSQL with retry policy for Egyptian network conditions
builder.Services.AddDbContext<StoreDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgreSQL"),
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null);

            // Use Egypt timezone
            npgsqlOptions.UseNodaTime();
        });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Configure Redis for caching (common in Egyptian deployments)
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "StationeryStore:";
});

// Configure JWT Authentication for Egyptian security standards
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // In production, should be true
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5) // Tolerance for Egyptian network latency
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Warning("Authentication failed for request {RequestPath}", context.HttpContext.Request.Path);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Log.Information("User {UserName} authenticated successfully",
                context.Principal?.Identity?.Name);
            return Task.CompletedTask;
        }
    };
});

// Configure Authorization with Egyptian roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("CashierOrAbove", policy =>
        policy.RequireRole("Cashier", "Manager", "Admin"));

    options.AddPolicy("EgyptianStoreAccess", policy =>
        policy.RequireClaim("BranchId"));
});

// Configure Swagger with Arabic support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Stationery Store Management System - Egypt",
        Version = "v1",
        Description = "API for managing stationery stores in Egypt with ETA e-invoicing integration",
        Contact = new OpenApiContact
        {
            Name = "Support - الدعم الفني",
            Email = "support@stationery-egypt.com",
            Url = new Uri("https://help.stationery-egypt.com")
        },
        License = new OpenApiLicense
        {
            Name = "Commercial License - ترخيص تجاري",
            Url = new Uri("https://license.stationery-egypt.com")
        }
    });

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Add Arabic/English documentation
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "StationeryStore.API.xml"), true);
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "StationeryStore.Application.xml"), true);

    // Add enum descriptions
    options.UseAllOfForInheritance();
    options.UseOneOfForPolymorphism();

    // Configure for Egyptian timezone
    options.MapType<DateTime>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date-time",
        Example = new OpenApiString("2024-01-01T10:30:00+02:00")
    });
});

// Add Egyptian-specific services
builder.Services.AddEgyptianServices(builder.Configuration);

// Configure CORS for Egyptian domains
builder.Services.AddCors(options =>
{
    options.AddPolicy("EgyptianFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000",
                "https://*.stationery-egypt.com",
                "https://stationery-egypt.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetPreflightMaxAge(TimeSpan.FromHours(1));
    });
});

// Configure JWT Bearer authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "StationeryStore Egypt v1");
        options.RoutePrefix = "api-docs";
        options.DocumentTitle = "Stationery Store Egypt API";
        options.DisplayRequestDuration();
        options.EnableDeepLinking();
        options.DefaultModelsExpandDepth(-1); // Hide schemas by default

        // Add Arabic translation option
        options.InjectJavascript("/swagger-custom.js");
    });

    // Seed Egyptian data in development
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
        await dbContext.Database.MigrateAsync();
        await EgyptianDataSeeder.SeedAsync(dbContext);
    }
}

// Apply Egyptian culture middleware
app.UseRequestLocalization();

// Use Egyptian timezone
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
TimeZoneInfo egyptTimeZone;
try
{
    egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
}
catch (TimeZoneNotFoundException)
{
    egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");
}

// Configure pipeline
app.UseHttpsRedirection();
app.UseCors("EgyptianFrontend");
app.UseAuthentication();
app.UseAuthorization();

// Add Egyptian health check endpoint
app.MapHealthChecks("/health", new()
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds,
                description = e.Value.Description
            }),
            timestamp = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                egyptTimeZone
            ).ToString("yyyy-MM-ddTHH:mm:sszzz")
        };
        await context.Response.WriteAsJsonAsync(response);
    }
});

// Add Egyptian-specific endpoints
app.MapGet("/egypt/info", () =>
{
    var info = new
    {
        Country = "Egypt",
        Currency = "EGP",
        DefaultVAT = 14.0,
        TimeZone = egyptTimeZone.Id,
        CurrentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone),
        SupportedLanguages = new[] { "ar", "en" },
        TaxAuthority = "Egyptian Tax Authority (ETA)",
        EInvoiceMandatory = true
    };
    return Results.Ok(info);
}).WithTags("Egypt").WithName("GetEgyptInfo");

app.MapControllers();

app.Run();

// Make Program class accessible for testing
public partial class Program { }