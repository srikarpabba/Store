using System.Text;
using Amazon.S3;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Email;
using Application.Abstractions.Storefront;
using Application.Abstractions.Storage;
using Application.Abstractions.Users;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.Authentication;
using Infrastructure.Authorization;
using Infrastructure.Database;
using Infrastructure.DomainEvents;
using Infrastructure.Email;
using Infrastructure.Storefront;
using Infrastructure.Storage;
using Infrastructure.Time;
using Infrastructure.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedKernel;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddServices()
            .AddIdentity()
            .AddDatabase(configuration)
            .AddCache(configuration)
            .AddHealthChecks(configuration)
            .AddAuthenticationInternal(configuration)
            .AddAuthorizationInternal()
            .AddEmail(configuration)
            .AddBackgroundJobs(configuration)
            .AddStorage(configuration);

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

        services.AddTransient<IStorefrontSectionService, StorefrontSectionService>();


        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection is not configured.");

        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseNpgsql(connectionString, npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException("Redis connection string is not configured.");

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
        });

        services.AddFusionCache()
                .WithDefaultEntryOptions(options =>
                {
                    options.Duration = TimeSpan.FromMinutes(5);
                    options.DistributedCacheDuration = TimeSpan.FromMinutes(5);
                })
                .WithSerializer(new FusionCacheSystemTextJsonSerializer())
                .WithBackplane(new RedisBackplane(new RedisBackplaneOptions
                {
                    Configuration = connectionString
                }))
                .AsHybridCache();

        return services;
    }

    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("DefaultConnection")!)
            .AddRedis(configuration.GetConnectionString("Redis")!);

        return services;
    }

    private static IServiceCollection AddAuthenticationInternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        IConfigurationSection jwtSection = configuration.GetSection(JwtOptions.SectionName);

        services.AddOptions<JwtOptions>()
            .Bind(jwtSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        JwtOptions jwtOptions = jwtSection.Get<JwtOptions>()
            ?? throw new InvalidOperationException("Jwt configuration is missing.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.RequireHttpsMetadata = false;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateIssuer = true,
                ValidAudience = jwtOptions.Audience,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddOptions<GoogleAuthOptions>()
            .Bind(configuration.GetSection(GoogleAuthOptions.SectionName));

        services.AddOptions<ClientOptions>()
            .Bind(configuration.GetSection(ClientOptions.SectionName));

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddSingleton<ITokenProvider, TokenProvider>();

        return services;
    }

    private static IServiceCollection AddEmail(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<EmailOptions>()
            .Bind(configuration.GetSection(EmailOptions.SectionName));

        services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        return services;
    }

    private static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options =>
                options.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection"))));

        services.AddHangfireServer();

        return services;
    }

    private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
    {
        services.AddAuthorization();

        services.AddScoped<PermissionProvider>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }

    private static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioOptions>(configuration.GetSection(MinioOptions.SectionName));

        MinioOptions options = configuration
            .GetSection(MinioOptions.SectionName)
            .Get<MinioOptions>()!;

        services.AddSingleton<IAmazonS3>(_ => CreateClient(options, options.Endpoint));

        // Presigned URLs embed the host they were signed for, so they must
        // be generated against the endpoint the browser will actually hit
        services.AddKeyedSingleton<IAmazonS3>(
            MinioFileStorage.PresignClientKey,
            (_, _) => CreateClient(
                options,
                string.IsNullOrWhiteSpace(options.PublicEndpoint)
                    ? options.Endpoint
                    : options.PublicEndpoint));

        services.AddScoped<IFileStorage, MinioFileStorage>();

        services.AddHostedService<MinioStartupValidator>();

        return services;
    }

    private static AmazonS3Client CreateClient(MinioOptions options, string endpoint)
    {
        return new AmazonS3Client(
            options.AccessKey,
            options.SecretKey,
            new AmazonS3Config
            {
                ServiceURL = endpoint,
                ForcePathStyle = true,
                UseHttp = endpoint.StartsWith(
                    "http://",
                    StringComparison.OrdinalIgnoreCase)
            });
    }
}
