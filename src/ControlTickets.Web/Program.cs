using ControlTickets.Infrastructure.Data;
using ControlTickets.Infrastructure.Services;
using ControlTickets.Web.Components;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registrar SQLite y Servicios
builder.Services.AddDbContext<TicketsDbContext>(options =>
    options.UseSqlite("Data Source=ControlTicketsIT.db"));

builder.Services.AddScoped<CsvImportService>();

var app = builder.Build();

// Crear base de datos y datos semilla automáticamente al arrancar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TicketsDbContext>();
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();