using FluentValidation;
using FluentValidation.AspNetCore;
using InvoiceManagementFinalProject.Config;
using InvoiceManagementFinalProject.Data;
using InvoiceManagementFinalProject.Mappings;
using InvoiceManagementFinalProject.Models;
using InvoiceManagementFinalProject.Services;
using InvoiceManagementFinalProject.Services.Interfaces;
using InvoiceManagementFinalProject.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace InvoiceManagementFinalProject.Extensions;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddSwagger(
        this IServiceCollection services
        )
    {
        services.AddControllers();

        services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Invoice Management",
            Description = "",
            Contact = new OpenApiContact
            {
                Name = "Akif Ismayilov",
                Email = "support@invoicemanagement.com"
            },
            License = new OpenApiLicense
            {
                Name = "MIT Licence",
                Url = new Uri("https://opensource.org/license/mit")
            }


        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);


        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = """
                JWT Suthorization header using the Bearer scheme. 
                Example: Authorization: Bearer {token}
                """,
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
            });
    }

    );
        return services;
    }


    public static IServiceCollection AddInvoiceAndCustomerDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration
                        .GetConnectionString("DefaultConnectionString");

        services.AddDbContext<HWDbContext>(
            options => options.UseSqlServer(connectionString)
            );
        return services;
    }

    public static IServiceCollection AddIdentityAndDb(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .Configure<JwtConfig>(configuration.GetSection(JwtConfig.SectionName));

        services.AddIdentity<AppUser, IdentityRole>(
    options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
    }

)
    .AddEntityFrameworkStores<HWDbContext>()
    .AddDefaultTokenProviders();
        return services;
    }

    public static IServiceCollection AddJwtAuthenticationAndAuthorization(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtConfig = new JwtConfig();

        configuration.GetSection(JwtConfig.SectionName).Bind(jwtConfig);


        services.AddAuthentication(
            options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            )
            .AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtConfig.Issuer,
                        ValidAudience = jwtConfig.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                }
            );

        services.AddAuthorization(options =>
        {
            options.AddPolicy("User",
                policy => policy.RequireRole("User"));
        });

        return services;
    }


    public static IServiceCollection AddCorsPolicy(
        this IServiceCollection services)
    {
        services.AddCors(
    optons =>
    {
        optons.AddDefaultPolicy(
            policy => policy.WithOrigins(
                "http://localhost:3000",
                "http://127.0.0.1:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            );
    }
    );
        return services;
    }



    public static IServiceCollection AddFluentValidation(
        this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<Program>();
        return services;
    }

    public static IServiceCollection AddAutoMapperAndOtherServices(
        this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAttachmentService, AttachmentService>();
        services.AddScoped<IFileStorage, LocalDiskStorage>();


        return services;

    }

}