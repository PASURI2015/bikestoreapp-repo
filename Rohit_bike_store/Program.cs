using Microsoft.EntityFrameworkCore;
using Rohit_bike_store.Models;
using Rohit_bike_store.Services;
using System.Text.Json.Serialization;
using AutoMapper;
using Rohit_bike_store.Profiles;
using Rohit_bike_store.Service;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.MaxDepth = 64; // Increase depth if needed
    });

builder.Services.AddDbContext<BikeStoreAuthContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("authconnection")));

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("BikeStoreUser")
    .AddEntityFrameworkStores<BikeStoreAuthContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});


builder.Services.AddAutoMapper(typeof(AutoMapperProfile)); // or the name of your Profile class

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddDbContext<RohitBikeStoreContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:myconnection"]);
});

builder.Services.AddScoped<IProduct, ProductServices>();
builder.Services.AddScoped<IStore, StoreServices>();
builder.Services.AddScoped<IStaff, StaffServices>();
builder.Services.AddScoped<ICategory, CategoryServices>();
builder.Services.AddScoped<IBrand, BrandServices>();
builder.Services.AddScoped<IOrder, OrderServices>();
builder.Services.AddScoped<ICustomer, CustomerServices>();
builder.Services.AddScoped<IOrderItem, OrderItemServices>();
builder.Services.AddScoped<IStock, StockServices>();
builder.Services.AddScoped<ITokenService, TokenServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add routing middleware
app.UseRouting();

// Add authentication and authorization middleware (order matters)
app.UseAuthentication(); // Ensure this comes before UseAuthorization
app.UseAuthorization();

// Map controllers to endpoints
app.MapControllers();

// Run the application
app.Run();



