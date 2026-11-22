using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shopping;
using Shopping.Services;
using Shopping.Settings;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>() ?? [];

// Add services to the container.

builder.Services
    .AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddProblemDetails();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Keycloak:Authority"];
    options.Audience = builder.Configuration["Keycloak:Audience"];
    if (!string.IsNullOrWhiteSpace(builder.Configuration["Keycloak:MetadataAddress"]))
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        options.MetadataAddress = builder.Configuration["Keycloak:MetadataAddress"];
#pragma warning restore CS8601 // Possible null reference assignment.
    }

    // Discovery normally requires https; Keycloak is plain http locally.
    options.RequireHttpsMetadata = !(builder.Configuration["Keycloak:AllowHttp"]?.ToLower() == "true");

    // Keep Keycloak's claim names ("sub", "preferred_username", "email")
    // instead of renaming them to long Microsoft claim-type URIs.
    options.MapInboundClaims = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = options.Authority,
        ValidateAudience = false,
        NameClaimType = "preferred_username", // makes User.Identity.Name work
        ClockSkew = TimeSpan.FromSeconds(30)  // tighter than the 5-minute default
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = ctx =>
        {
            ctx.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>()
                .LogWarning(ctx.Exception, "JWT rejected");
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Policies.HouseholdMember, p => p.AddRequirements(new HouseholdMemberRequirement()));
builder.Services.AddScoped<IAuthorizationHandler, HouseholdMemberHandler>();

builder.Services.AddDbContext<ShoppingDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("ShoppingDb"))
    .UseSnakeCaseNamingConvention()
);

builder.Services.Configure<IdentitySettings>(builder.Configuration.GetSection("IdentitySettings"));
builder.Services.AddHttpClient<IIdentityClient, IdentityClient>((sp, client) =>
{
    client.BaseAddress = new Uri(sp.GetRequiredService<IOptions<IdentitySettings>>().Value.BaseUrl);
}).AddStandardResilienceHandler();

builder.Services.Configure<InventorySettings>(builder.Configuration.GetSection("InventorySettings"));
builder.Services.AddHttpClient<IInventoryClient, InventoryClient>((sp, client) =>
{
    client.BaseAddress = new Uri(sp.GetRequiredService<IOptions<InventorySettings>>().Value.BaseUrl);
}).AddStandardResilienceHandler();

builder.Services.AddHealthChecks()
    .AddNpgSql(sp => sp.GetRequiredService<IConfiguration>().GetConnectionString("ShoppingDb")!
);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
