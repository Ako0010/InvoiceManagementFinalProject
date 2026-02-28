using InvoiceManagementFinalProject.Data;

namespace InvoiceManagementFinalProject.Extensions;

public static class PipelineExtensions
{
    public static WebApplication UseApplicationPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoice API v1");
                options.RoutePrefix = string.Empty;
                options.DisplayRequestDuration();
                options.EnableFilter();
                options.EnableDeepLinking();
                options.EnableTryItOutByDefault();
            });
        }

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
    public static async Task EnsureRolesSeededAsync(
        this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        await RoleSeeder.SeedRolesAsync(scope.ServiceProvider);
    }
}