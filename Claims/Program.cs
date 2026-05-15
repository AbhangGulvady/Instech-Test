using Claims.Extensions;

var builder = WebApplication.CreateBuilder(args);

var (sqlContainer, mongoContainer) = await TestContainersServiceExtensions.StartTestContainersAsync(builder.Configuration);

builder.Services.AddControllersWithJsonOptions();
builder.Services.AddDbContexts(builder.Configuration, sqlContainer.GetConnectionString(), mongoContainer.GetConnectionString());
builder.Services.AddScopedServices();
builder.Services.AddSingletonServices();
builder.Services.AddAuditMessaging(builder.Configuration);   // ← feature-flag added here

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MigrateAuditContext();

app.Run();

public partial class Program { }
