using gerep_core;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSystemd();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddRouting(option =>
{
    option.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
    option.LowercaseUrls = true;
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Scripts")),
    RequestPath = "/Scripts"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Content")),
    RequestPath = "/Content"
});


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();
app.UseSession();


app.MapDefaultControllerRoute();

app.MapControllerRoute(
  name: "DefaultApi",
  pattern: "api/{controller:slugify=Home}/{id?}"
);

app.MapControllerRoute(
    name: "Default",
    pattern: "{controller:slugify=Home}/{action:slugify=Index}/{id?}",
    defaults: new { controller = "Home", action = "Index"}
);

app.MapRazorPages();

app.Run();