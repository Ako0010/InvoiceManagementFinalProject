using InvoiceManagementFinalProject.Extensions;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddSwagger()
                .AddInvoiceAndCustomerDbContext(builder.Configuration)
                .AddIdentityAndDb(builder.Configuration)
                .AddJwtAuthenticationAndAuthorization(builder.Configuration)
                .AddFluentValidation()
                .AddAutoMapperAndOtherServices();

QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

await app.EnsureRolesSeededAsync();

app.UseApplicationPipeline();

app.Run();