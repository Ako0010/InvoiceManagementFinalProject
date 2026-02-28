using InvoiceManagementFinalProject.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddSwagger()
                .AddInvoiceAndCustomerDbContext(builder.Configuration)
                .AddIdentityAndDb(builder.Configuration)
                .AddJwtAuthenticationAndAuthorization(builder.Configuration)
                .AddFluentValidation()
                .AddAutoMapperAndOtherServices();

var app = builder.Build();

await app.EnsureRolesSeededAsync();

app.UseApplicationPipeline();

app.Run();