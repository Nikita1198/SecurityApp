using Microsoft.AspNetCore.Authorization;
using SecurityApp.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly",
        policy => policy.RequireClaim("Admin"));

    options.AddPolicy("MustBelongToHRDepartment",
       policy => policy.RequireClaim("Department", "HR"));

    options.AddPolicy("HRManagerOnly", policy => policy
        .RequireClaim("Department", "HR")
        .RequireClaim("Manager")
        .Requirements.Add(new HRManagerProbationRequirment(3)));
});

builder.Services.AddSingleton<IAuthorizationHandler, HRManagerProbationRequirmentHandler>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//If we want to work with Authorization we must rigth a UseAuthentication ->
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
