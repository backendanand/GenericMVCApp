using GenericMVCApp.Data;
using GenericMVCApp.Interfaces;
using GenericMVCApp.Repositories;
using GenericMVCApp.ServiceModels;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Bind settings from appsettings.json
//var cookieSettings = builder.Configuration.GetSection("Authentication:Cookie").Get<CookieSettings>();
builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", options =>
{
    options.LoginPath = "/Auth/Login"; // Path to login page
    options.LogoutPath = "/Auth/Logout"; // Path to logout
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Session expiration time
    options.SlidingExpiration = true; // Extend session expiration with activity
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

// Add services to the container.
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAuthRepository, AuthRepository>();


var app = builder.Build();
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

app.Run();
