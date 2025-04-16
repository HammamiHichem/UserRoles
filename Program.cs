using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserRoles.Data;
using UserRoles.Models;
using UserRoles.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.ConfigureApplicationCookie(options =>
{
    // Redirection après connexion réussie
    options.LoginPath = "/Account/Login"; // Si nécessaire, définir une page de login
    options.AccessDeniedPath = "/Home/AccessDenied"; // Page en cas d'accès refusé

    // Permet d'empêcher la connexion automatique de l'administrateur
    options.SlidingExpiration = true; // Rafraîchit la session si l'utilisateur reste actif
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Définit une durée d'expiration du cookie (ex. 30 minutes)

    // Cette redirection n'est plus nécessaire, car elle est gérée par l'authentification elle-même.
    // options.Events.OnRedirectToAccessDenied = context =>
    // {
    //     if (context.Request.Path.StartsWithSegments("/Home/Admin"))
    //     {
    //         context.Response.Redirect("/Home/Admin");
    //     }
    //     return Task.CompletedTask;
    // };
});

// Configure MySQL DbContext with connection string from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
    .EnableDetailedErrors() // Optionnel : pour plus de détails lors du débogage des erreurs SQL
    .EnableSensitiveDataLogging(); // Optionnel : pour activer l'affichage des données sensibles (utile uniquement en dev)
});

// Configure Identity with custom user and role models
builder.Services.AddIdentity<Users, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Seed the database with roles and the admin user
await SeedService.SeedDatabase(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles(); // Serve static files like CSS, JS, etc. after UseRouting()

app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization(); // Enable authorization middleware

// Configure the default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
