using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using POSDb.Data; // apna namespace
using POSDb.EntityModels;
using POSModels.Models;
using POSModels.Services;
using POSModels.ViewModels; // jahan ApplicationUser hai

var builder = WebApplication.CreateBuilder(args);

// 🔹 DB Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Identity with ApplicationUser
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();


// 🔹 Cookie Configuration (🔥 important)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "POS_SYSTEM";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(3600);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});


builder.Services.AddScoped<ILoginViewModels, LoginViewModels>();
builder.Services.AddScoped<IHomeService, HomeViewModel>();
builder.Services.AddScoped<ISelectItemService, SelectViewModel>();
builder.Services.AddScoped<IPurchaseService, PurchaseViewModel>();
builder.Services.AddScoped<IBillingService, BillingService>();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();


builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}

// 🔹 Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔹 IMPORTANT
app.UseAuthentication();
app.UseAuthorization();

// 🔹 Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapRazorPages();

app.Run();