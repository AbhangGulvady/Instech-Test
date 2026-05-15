using Claims.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Claims.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MigrateAuditContext(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
        context.Database.Migrate();
        return app;
    }
}