using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    c => { c.LoginPath = "/music/Login"; c.AccessDeniedPath = "/music/AccessDenied"; });
//builder.Services.AddAuthorization(config =>
//{
//    config.AddPolicy("up", policyBuilder =>
//    {
//        policyBuilder.RequireClaim(ClaimTypes.Role, "M");



//    });
//});

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
    pattern: "{controller=music}/{action=home}/{id?}");

app.Run();
