using _LAB__QC_HQ.Data;
using _LAB__QC_HQ.Interfaces;
using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Keep true to test email confirmation
    options.SignIn.RequireConfirmedEmail = false;
    options.User.RequireUniqueEmail = true;

    // Optional: Relax password rules for testing
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
}).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddSingleton<IEmailSender, GmailEmailSender>();
builder.Services.AddControllersWithViews();
//added services for content
builder.Services.AddScoped<IContentService, ContentService>();
builder.Services.AddScoped<IKnowHowService, KnowHowService>();
builder.Services.AddScoped<IContentAuthorizationService, ContentAuthorizationService>();
builder.Services.AddScoped<IItemService, ItemService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await SeedRolesAndAdmin(serviceProvider);
}

app.UseHttpsRedirection();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.Use(async (context, next) =>
{
    var path = context.Request.Path;

    // Log Identity-related requests
    if (path.StartsWithSegments("/Identity/Account"))
    {
        Console.WriteLine($"🔍 [{DateTime.Now:HH:mm:ss}] Identity Request: {context.Request.Method} {path}");

        if (context.Request.Method == "POST" &&
            (path.Value.Contains("ForgotPassword") || path.Value.Contains("Register")))
        {
            try
            {
                // Enable buffering to read form data
                context.Request.EnableBuffering();

                // Create a copy of the stream
                using var memoryStream = new MemoryStream();
                await context.Request.Body.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Read from the copy
                using var reader = new StreamReader(memoryStream);
                var body = await reader.ReadToEndAsync();

                // Reset the original stream position
                context.Request.Body.Position = 0;

                Console.WriteLine($"   Form data: {body}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error reading form: {ex.Message}");
            }
        }
    }

    await next();
});

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();






app.Run();


async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Create roles if they don't exist
    string[] roleNames = { "Admin", "User"};

    foreach (var roleName in roleNames)
    {
        var roleExists = await roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
            Console.WriteLine($"Created role: {roleName}");
        }
    }

    // Create default admin user if it doesn't exist
    var adminEmail = "arapstef3@gmail.com";
    var adminUser = await userManager.FindByEmailAsync("arapstef3@gmail.com");
if (adminUser == null)
{
    adminUser = new ApplicationUser
    {
        UserName = "arapstef3@gmail.com",
        Email = "arapstef3@gmail.com",
        FirstName = "Admin",
        LastName = "User",
        // ... other properties
    };
    
    var result = await userManager.CreateAsync(adminUser, "YourPasswordHere!");
    
    if (result.Succeeded)
    {
        // THIS LINE IS CRITICAL:
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}
else
{
    // User exists, but maybe doesn't have role
    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}
}