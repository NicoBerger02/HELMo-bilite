using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Helmobilite.Models;
using Helmobilite.Data;
using Helmobilite.Services;

var builder = WebApplication.CreateBuilder(args);

var builderConfiguration = new ConfigurationBuilder()
	.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builderConfiguration.Build();

// Add services to the container.
builder.Services.AddDbContext<HelmobiliteDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("default")));

builder.Services.AddRazorPages();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<HelmobiliteDbContext>()
    .AddErrorDescriber<HelmobiliteErrorDescriber>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

builder.Services.AddScoped<IImageService, ImageService>();

var app = builder.Build();

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
var userManager = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
var roleManager = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
var dbContext = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<HelmobiliteDbContext>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
	app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

DataInitializer.SeedData(userManager, roleManager, dbContext).Wait();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
