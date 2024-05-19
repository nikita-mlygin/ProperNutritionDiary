using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ProperNutritionDiary.Product.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddAuthorization();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(opt =>
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

        services
            .AddAuthentication(options =>
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
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            configuration["Jwt:Key"] ?? throw new InvalidConfigurationException()
                        )
                    ),
                    ClockSkew = TimeSpan.FromSeconds(1),
                };

                options.IncludeErrorDetails = true;
            });

        services
            .AddAuthorizationBuilder()
            .AddPolicy("canCreateProduct", c => c.RequireRole("plain", "admin", "app"))
            .AddPolicy("canViewProduct", c => c.RequireRole("guest", "plain", "admin", "app"))
            .AddPolicy("guest", c => c.RequireRole("guest", "plain"))
            .AddPolicy("plain", c => c.RequireRole("plain"));

        var cat = new DependencyContextAssemblyCatalog([typeof(DependencyInjection).Assembly]);

        services.AddCarter(cat);

        return services;
    }

    public static WebApplication AddPresentation(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapCarter();

        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}
