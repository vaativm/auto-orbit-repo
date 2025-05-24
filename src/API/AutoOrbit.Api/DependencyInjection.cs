using AutoOrbit.Api.Application.Abstractions;
using AutoOrbit.Api.Application.Services;
using AutoOrbit.Api.Shared.Models;
using Firebase.Auth.Providers;
using Firebase.Auth;
using FirebaseAdmin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.Reflection;
using AutoOrbit.Api.Shared.Infrastructure;
using FluentValidation.AspNetCore;
using FluentValidation;
using AutoOrbit.Api.Infrastructure.Time;
using AutoOrbit.Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Refit;
using AutoOrbit.Api.Shared.RestClients;
using System.Text.Json;
using JorgeSerrano.Json;
using AutoOrbit.Api.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AutoOrbit.Api;

internal static class DependencyInjection
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSettings(configuration)
            .AddHttpContextAccessor()
            .AddGlobalExeptionHandler()
            .ConfigureCors()
            .ConfigFluentValidation()
            .AddFirebaseAuthentication(configuration)
            .AddDomainServices(configuration)
            .AddDatabase(configuration);

        return services;
    }

    public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FirebaseOptions>(configuration.GetSection(FirebaseOptions.SectionName));
        services.Configure<ImageLocationOptions>(configuration.GetSection(ImageLocationOptions.SectionName));

        return services;
    }
    private static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IUserProfilesService, UserProfilesService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IPartsService, PartsService>();
        services.AddScoped<IOrganizationService, OrganizationService>();

        services.AddRefitClient<IFirebaseApi>(_ => new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
            {
                PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy()
            })
        })
            .ConfigureHttpClient(x => x.BaseAddress = new Uri($"{configuration["Firebase:BaseUrl"]}"))
            .AddFaultHandlingPolicy();

        return services;
    }

    private static IServiceCollection AddGlobalExeptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    private static IServiceCollection ConfigFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);

        services.AddFluentValidationAutoValidation(options => options.DisableDataAnnotationsValidation = true);

        return services;
    }
    private static IServiceCollection ConfigureCors(this IServiceCollection services)
        => services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("X-Pagination")
            ));

    private static IServiceCollection AddFirebaseAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        FirebaseOptions? firebaseOptions = configuration.GetSection(FirebaseOptions.SectionName).Get<FirebaseOptions>();

        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", firebaseOptions!.CredentialsFilePath);

        services.AddSingleton(FirebaseApp.Create());

        string credentials = File.ReadAllText(firebaseOptions!.CredentialsFilePath!);
        IList<SecurityKey> signinKeys = new JsonWebKeySet(credentials).GetSigningKeys();

        services.AddSingleton(new FirebaseAuthClient(new FirebaseAuthConfig
        {
            ApiKey = firebaseOptions!.ApiKey,
            AuthDomain = firebaseOptions.AuthDomain,
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        }));

        services.AddSingleton<IFirebaseAuthService, FirebaseAuthService>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                options.Authority = $"https://securetoken.google.com/{firebaseOptions.ProjectId}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://securetoken.google.com/{firebaseOptions.ProjectId}",
                    ValidateAudience = true,
                    ValidAudience = firebaseOptions.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = signinKeys
                };
            });

        return services;
    }

    private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
    {
        services.AddAuthorization();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("Database");

        services.AddDbContextPool<AutoOrbitDbContext>(
            options => options
                .UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default);
                    npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
                })
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IAutoOrbitDbContext>(sp => sp.GetRequiredService<AutoOrbitDbContext>());

        return services;
    }

    public static void ConfigSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(s => {
            string file = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            s.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, file));

            s.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "AutoOrbit Api",
                Version = "v1",
                Description = "An api for managing automotive inventory"
            });

            var security = new OpenApiSecurityScheme
            {
                Name = HeaderNames.Authorization,
                Type = SecuritySchemeType.Http,
                In = ParameterLocation.Header,
                Description = "JWT Authorization header",
                BearerFormat = "JWT",
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            s.AddSecurityDefinition(security.Reference.Id, security);

            s.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    security,
                    Array.Empty<string>()
                }
            });
        });
    }

}
