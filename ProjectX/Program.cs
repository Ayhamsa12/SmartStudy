using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectX.Data;
using ProjectX.Helpers;
using ProjectX.Hup;
using ProjectX.Interface;
using ProjectX.Models;
using ProjectX.Repository;
using ProjectX.Services;
using SendGrid.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<IReferenceRepository, ReferenceRepository>();

builder.Services.AddScoped<IGalaxsyRepository, GalaxsyRepository>();
builder.Services.AddScoped<ICloudService, CloudService>();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnictionString"));
});
builder.Services.AddIdentity<Users, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

builder.Services.Configure<IdentityOptions>(Options =>
{
    Options.Password.RequireUppercase = false;
    Options.Password.RequiredLength = 8;
   

});
builder.Services.AddSignalR();
builder.Services.AddAuthorization();
builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});
builder.Services.Configure<EmailSenderService>(builder.Configuration.GetSection("Email"));
builder.Services.AddSendGrid(options =>
{
    options.ApiKey = builder.Configuration.GetSection("Email").GetValue<string>("ApiKey");
});
builder.Services.AddScoped<IEmailSender, EmailSenderService>();

var app = builder.Build();
if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
     Seed.SeedUsersAndRoles(app);
}

// Configure the HTTP request pipeline.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ChatHub>("/chatHub");
app.Run();
