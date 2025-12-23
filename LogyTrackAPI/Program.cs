using LogyTrackAPI.Data;
using LogyTrackAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using LogyTrackAPI.Infrastructure.Data.Drivers;
using LogyTrackAPI.Infrastructure.Data.Vehiclas;
using LogyTrackAPI.Infrastructure.Data.Customers;
using LogyTrackAPI.Infrastructure.Data.Vehicles;
using LogyTrackAPI.Infrastructure.Data.Products;

var builder = WebApplication.CreateBuilder(args);

//// ===== ADD BOTH DbContexts =====
//builder.Services.AddDbContext<LogyTrackContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== ADD DAPPER CONTEXT =====
builder.Services.AddScoped<DapperContext>();

// ===== REGISTER REPOSITORIES =====
builder.Services.AddScoped<IDriverRepository, DriverServices>();
builder.Services.AddScoped<IVehicleRepository, VehicleServices>();
builder.Services.AddScoped<IUserRepository, UserServices>();
builder.Services.AddScoped<IProductRepository, ProductServices>();

// Add AuthService
builder.Services.AddScoped<AuthService>();

// ===== ADD CONTROLLERS WITH VALIDATION DISABLED =====
builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
})
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// ===== ADD JWT Authentication =====
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];

if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
{
    throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long");
}

var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// ===== ADD AUTHORIZATION =====
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin", "admin"));
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Middleware order is CRITICAL
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();