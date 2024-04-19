using System.Text;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProperNutritionDiary.User.Api.Database;
using ProperNutritionDiary.User.Api.User.Tokens;

const string MainCorsPolicy = "mainCors";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppCtx>(conf =>
{
    conf.UseSqlServer(builder.Configuration.GetConnectionString("mssql"));
    conf.EnableSensitiveDataLogging();
});

builder.Services.AddScoped<TokenGenerator>();
builder.Services.AddScoped<UserService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        }
    );

    opt.AddSecurityRequirement(
        new OpenApiSecurityRequirement
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
        }
    );

    opt.CustomSchemaIds(type => type.FullName);
});

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"] ?? throw new InvalidConfigurationException()
                )
            ),
            ClockSkew = TimeSpan.FromSeconds(1),
        };

        options.IncludeErrorDetails = true;
    });

builder
    .Services.AddAuthorizationBuilder()
    .AddPolicy("guest", c => c.RequireRole("guest", "plain"))
    .AddPolicy("plain", c => c.RequireRole("plain"));

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: MainCorsPolicy,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

builder.Services.AddCarter();

var app = builder.Build();

app.UseCors(MainCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapGet("/", () => "Hello World!");

app.MapCarter();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
