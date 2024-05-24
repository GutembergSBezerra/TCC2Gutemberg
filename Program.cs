using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAuthentication("Cookies")
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
        options.SlidingExpiration = true;
    });

builder.Services.AddSession(); // Add this line

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Add this line
app.UseAuthentication();
app.UseAuthorization();

// Redirect root URL to the Login page if the user is not authenticated
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/" && (context.User?.Identity == null || !context.User.Identity.IsAuthenticated))
    {
        context.Response.Redirect("/Login");
        return;
    }
    await next.Invoke();
});

app.MapRazorPages();

// Logout route
app.MapPost("/Logout", async context =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    context.Response.Redirect("/Login");
});

app.Run();