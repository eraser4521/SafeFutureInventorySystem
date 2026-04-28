using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafeFutureInventorySystem.Data;
using SafeFutureInventorySystem.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AuthConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var inventoryDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    inventoryDb.Database.Migrate();
    await EnsureInventoryItemsColumnsAsync(inventoryDb);

    var authDb = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    authDb.Database.Migrate();

    await EnsureAspNetUsersColumnsAsync(authDb);

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "Admin", "Volunteer" };
    foreach (var r in roles)
    {
        if (!await roleManager.RoleExistsAsync(r))
            await roleManager.CreateAsync(new IdentityRole(r));
    }

    var adminEmail = "admin@safefuture.com";
    var adminPass = "admin1234";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
       adminUser = new ApplicationUser
{
    UserName = adminEmail,
    Email = adminEmail,
    EmailConfirmed = true,
    FirstName = "Admin",
    LastName = "User",
    MustChangePassword = false,
    PasswordSetByAdmin = false
};

        var createResult = await userManager.CreateAsync(adminUser, adminPass);
        if (createResult.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Admin");
    }
    else
    {
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Error", "?code={0}");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.Use(async (context, next) =>
{
    if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
    {
        var path = context.Request.Path.Value ?? "";

        var allowed =
            path.StartsWith("/Account/ForceChangePassword", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/Account/Logout", StringComparison.OrdinalIgnoreCase);

        if (!allowed)
        {
            var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.GetUserAsync(context.User);

            if (user != null && user.MustChangePassword)
            {
                context.Response.Redirect("/Account/ForceChangePassword");
                return;
            }
        }
    }

    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

static async Task EnsureInventoryItemsColumnsAsync(ApplicationDbContext inventoryDb)
{
    var connection = inventoryDb.Database.GetDbConnection();
    await connection.OpenAsync();

    try
    {
        var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        await using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "PRAGMA table_info('InventoryItems')";
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                columns.Add(reader.GetString(1));
            }
        }

        if (!columns.Contains("LowStockThreshold"))
        {
            await inventoryDb.Database.ExecuteSqlRawAsync(
                "ALTER TABLE InventoryItems ADD COLUMN LowStockThreshold INTEGER NOT NULL DEFAULT 0");
        }
    }
    finally
    {
        await connection.CloseAsync();
    }
}

static async Task EnsureAspNetUsersColumnsAsync(AuthDbContext authDb)
{
    var connection = authDb.Database.GetDbConnection();
    await connection.OpenAsync();

    try
    {
        var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        await using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "PRAGMA table_info('AspNetUsers')";
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                columns.Add(reader.GetString(1));
            }
        }

        if (!columns.Contains("MustChangePassword"))
        {
            await authDb.Database.ExecuteSqlRawAsync(
                "ALTER TABLE AspNetUsers ADD COLUMN MustChangePassword INTEGER NOT NULL DEFAULT 0");
        }

        if (!columns.Contains("PasswordSetByAdmin"))
        {
            await authDb.Database.ExecuteSqlRawAsync(
                "ALTER TABLE AspNetUsers ADD COLUMN PasswordSetByAdmin INTEGER NOT NULL DEFAULT 0");
        }

        if (!columns.Contains("TemporaryPasswordIssuedAtUtc"))
        {
            await authDb.Database.ExecuteSqlRawAsync(
                "ALTER TABLE AspNetUsers ADD COLUMN TemporaryPasswordIssuedAtUtc TEXT NULL");
        }
    }
    finally
    {
        await connection.CloseAsync();
    }
}
