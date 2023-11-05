using learn.it.Models;
using Microsoft.EntityFrameworkCore;

/*
 * Modifications to launchSettings.json (found in Properties):
 * In all profiles, removed "ASPNETCORE_HOSTINGSTARTUPASSEMBLIES": "Microsoft.AspNetCore.SpaProxy" from environmentVariables
 * Changed launchBrowser to false in all profiles
 * These changes should be reverted when work on frontend begins.
 */

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<LearnitDbContext>(options =>
    options.UseSqlServer(
        "Data Source=localhost,1433;Initial Catalog=learnitdb;Persist Security Info=True;User ID=sa;Password=!root123456"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
