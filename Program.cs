using Microsoft.EntityFrameworkCore;
using GestionnaireFootball.Data;
using GestionnaireFootball.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuration de la session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session de 30 minutes
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configuration d'Entity Framework Core avec MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// CONFIGURATION DU PIPELINE HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Activation de la session (DOIT être après UseRouting et avant MapControllerRoute)
app.UseSession();

//  INITIALISATION DE LA BASE DE DONNÉES
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Vérifier si la base a déjà été initialisée
        if (!context.Utilisateurs.Any())
        {
            Console.WriteLine(" Initialisation de la base de données...");
            DbInitializer.Initialize(context);
            Console.WriteLine(" Base de données initialisée avec succès !");
        }
        else
        {
            Console.WriteLine(" Base de données déjà initialisée.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, " Erreur lors de l'initialisation de la base de données");
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();