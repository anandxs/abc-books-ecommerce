using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using AbcBooks.Utilities;
using AbcBooks.DataAccess;
using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.DataAccess.Repository;
using AbcBooks.Models;
using Stripe;
using AbcBooks.DataAccess.DbInitializer;
using Serilog;
using AbcBooks.Utilities.Interfaces;
using Microsoft.Extensions.Logging.AzureAppServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
	.AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")!));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(config =>
{
	config.SignIn.RequireConfirmedEmail = true;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = $"/Identity/Account/Login";
	options.LogoutPath = $"/Identity/Account/Logout";
	options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
builder.Services.AddAuthentication()
	.AddGoogle(options =>
{
	options.ClientId = Environment.GetEnvironmentVariable("google_clientid")!;
	options.ClientSecret = Environment.GetEnvironmentVariable("google_clientsecret")!;
});
builder.Services.Configure<SmtpConfig>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
	options.ValidationInterval = TimeSpan.FromMinutes(1);
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(100);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IImageModifier, ImageModifier>();
//builder.Services.AddTransient<ISmsSender, SmsSender>();
builder.Services.AddAutoMapper(typeof(AutoMapperConfig));
builder.Host.UseSerilog((context, config) =>
	config.ReadFrom.Configuration(context.Configuration));
builder.Logging.AddAzureWebAppDiagnostics();
builder.Services.Configure<AzureFileLoggerOptions>(options =>
{
	options.FileName = "logs-";
	options.FileSizeLimit = 1024 * 50;
	options.RetainedFileCountLimit = 5;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
SeedDatabase();
app.UseAuthentication();
app.UseSession();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();

void SeedDatabase()
{
	using (var scope = app.Services.CreateScope())
	{
		var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
		dbInitializer.Initialize();
	}
}
