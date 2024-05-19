using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProperNutritionDiary.UserMenuApi.Db;
using ProperNutritionDiary.UserMenuApi.Logger;
using ProperNutritionDiary.UserMenuApi.Product;
using ProperNutritionDiary.UserMenuApi.UserMenu;
using ProperNutritionDiary.UserMenuApi.UserMenu.Edamam;
using Refit;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder
    .Configuration.SetBasePath(System.IO.Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{env}.json", optional: true)
    .AddEnvironmentVariables("PROPER_NUTRITION_DIARY");

builder.Services.AddScoped<EdamamConverter>();
builder.Services.AddScoped<UserMenuService>();
builder.Services.AddScoped<MenuConfigurationService>();

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

builder.Services.AddDbContext<AppCtx>(conf =>
{
    conf.UseSqlServer(builder.Configuration.GetConnectionString("mssql"));
});

builder.Services.AddCarter(new DependencyContextAssemblyCatalog([typeof(UserMenuModule).Assembly]));

builder
    .Services.AddRefitClient<IProductApi>()
    .ConfigureHttpClient(cc =>
    {
        cc.BaseAddress = new Uri(new Uri(builder.Configuration["GatewayPath"]!), "product-api");
        cc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            builder.Configuration["ServerToken"]!
        );

        Console.WriteLine(new Uri(new Uri(builder.Configuration["GatewayPath"]!), "product-api"));
        Console.WriteLine(
            new AuthenticationHeaderValue("Bearer", builder.Configuration["ServerToken"]!)
        );
    })
    .AddHttpMessageHandler(serviceProvider => new HttpLoggingHandler(
        serviceProvider.GetRequiredService<ILogger<HttpLoggingHandler>>()
    ))
    .ConfigurePrimaryHttpMessageHandler(
        () =>
            new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true
            }
    );

builder
    .Services.AddRefitClient<IEdamamMenuApi>(
        new RefitSettings() { ContentSerializer = new NewtonsoftJsonContentSerializer() }
    )
    .ConfigureHttpClient(cc =>
    {
        cc.BaseAddress = new Uri(builder.Configuration["Edamam:MenuUrl"]!);
        cc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            builder.Configuration["Edamam:ApiKeyMenu"]
        );
        cc.DefaultRequestHeaders.Add("Edamam-Account-User", builder.Configuration["Edamam:UserId"]);
    });

builder
    .Services.AddRefitClient<IEdamamRecipeApi>(
        new RefitSettings() { ContentSerializer = new NewtonsoftJsonContentSerializer() }
    )
    .ConfigureHttpClient(cc =>
    {
        cc.BaseAddress = new Uri(builder.Configuration["Edamam:RecipeUrl"]!);
        cc.DefaultRequestHeaders.Add("Edamam-Account-User", builder.Configuration["Edamam:UserId"]);
    });

builder.Host.UseSerilog(
    (context, configuration) => configuration.ReadFrom.Configuration(context.Configuration)
);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapCarter();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapGet(
    "/{id}",
    async (IProductApi papi, [FromRoute] Guid id) => TypedResults.Ok(await papi.GetProductById(id))
);

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
