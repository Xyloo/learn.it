using System.Security.Claims;
using System.Text;
using learn.it.Models;
using learn.it.Repos;
using learn.it.Repos.Interfaces;
using learn.it.Services;
using learn.it.Services.Interfaces;
using learn.it.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Hellang.Middleware.ProblemDetails;
using learn.it.Exceptions;
using Microsoft.AspNetCore.Mvc;

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
        builder.Configuration.GetConnectionString("MsSqlConnection")
        ));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "learn.it", Version = "v1" });
    //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddProblemDetails(options =>
{
    options.Map<NotFoundException>(ex => new ProblemDetails
    {
        Title = "Not found",
        Detail = ex.Message,
        Status = StatusCodes.Status404NotFound
    });

    options.Map<AlreadyExistsException>(ex => new ProblemDetails
    {
        Title = "Data already exists",
        Detail = ex.Message,
        Status = StatusCodes.Status409Conflict
    });

    options.Map<InvalidInputDataException>(ex => new ProblemDetails
    {
        Title = "Invalid data",
        Detail = ex.Message,
        Status = StatusCodes.Status400BadRequest
    });

    options.Map<UnauthorizedAccessException>(ex => new ProblemDetails
    {
        Title = "User unauthorized",
        Detail = ex.Message,
        Status = StatusCodes.Status401Unauthorized
    });

    options.Map<ForbiddenAccessException>(ex => new ProblemDetails
    {
        Title = "Access forbidden",
        Detail = ex.Message,
        Status = StatusCodes.Status403Forbidden
    });

    options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
});

builder.Services.AddLogging(b =>
{
    b.AddConsole();
    b.AddDebug();
});

builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthMiddlewareResultHandler>();

JwtSettings.Key = builder.Configuration.GetSection("JwtSettings")["Key"] ?? throw new NullReferenceException("JwtSettings:Key not found in appsettings.json");
JwtSettings.Audience = builder.Configuration.GetSection("JwtSettings")["Audience"] ?? throw new NullReferenceException("JwtSettings:Audience not found in appsettings.json");
JwtSettings.Issuer = builder.Configuration.GetSection("JwtSettings")["Issuer"] ?? throw new NullReferenceException("JwtSettings:Issuer not found in appsettings.json");

builder.Services.AddAuthentication(auth => {
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt => {
    jwt.RequireHttpsMetadata = false;
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = JwtSettings.Issuer,
        ValidAudience = JwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtSettings.Key)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admins", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Users", policy => policy.RequireAssertion(context => context.User.HasClaim( c => 
        c is { Type: ClaimTypes.Role, Value: "Admin" } or { Type: ClaimTypes.Role, Value: "User"} )));
});


builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IPermissionsRepository, PermissionsRepository>();
builder.Services.AddScoped<ILoginsRepository, LoginsRepository>();
builder.Services.AddScoped<IGroupsRepository, GroupsRepository>();
builder.Services.AddScoped<IStudySetsRepository, StudySetsRepository>();
builder.Services.AddScoped<IFlashcardsRepository, FlashcardsRepository>();

builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<ILoginsService, LoginsService>();
builder.Services.AddScoped<IGroupsService, GroupsService>();
builder.Services.AddScoped<IStudySetsService, StudySetsService>();
builder.Services.AddScoped<IFlashcardsService, FlashcardsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            swaggerDoc.Servers = new List<OpenApiServer> { new() { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
        });
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("v1/swagger.json", "learn.it API V1");
    });
}

//app.UseHttpsRedirection();
app.UseProblemDetails();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<JwtBlacklistMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
