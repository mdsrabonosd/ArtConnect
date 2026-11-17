using ArtConnect.DataAccess;
using ArtConnect.DataAccess.Seed;
using ArtConnect.Entities;
using ArtConnect.Facade;
using ArtConnect.Framework;
using ArtConnect.Web.UI.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // Navigation properties like Category are not posted from forms, so do not treat them as required.
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

// Dependency Injection: DbContext + repositories + facades
builder.Services.AddArtConnectFacade(builder.Configuration);

// ASP.NET Core Identity (users, roles, password hashing, login)
builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/" + UrlRewrite.Account.Login;
    options.LogoutPath = "/" + UrlRewrite.Account.Logout;
    options.AccessDeniedPath = "/" + UrlRewrite.Account.AccessDenied;

    options.Cookie.Name = "ArtConnect.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;

    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
});

// Check the security stamp on every request so a blocked user is signed out immediately.
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IImageUploadService, ImageUploadService>();

// Session is used for the shopping cart
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "ArtConnect.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromHours(2);
});

var app = builder.Build();

// Seed roles, default admin and default categories.
// The database must exist first: run Add-Migration and Update-Database.
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

    try
    {
        var adminEmail = builder.Configuration["SeedAdmin:Email"] ?? "admin@artconnect.com";
        var adminPassword = builder.Configuration["SeedAdmin:Password"] ?? "Admin@12345";
        var adminName = builder.Configuration["SeedAdmin:FullName"] ?? "System Admin";

        await DbSeeder.SeedAsync(scope.ServiceProvider, adminEmail, adminPassword, adminName);
        logger.LogInformation("Database seed completed.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database seed failed. Run Update-Database first, then restart the app.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
