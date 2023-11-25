using Microsoft.EntityFrameworkCore;
using Projekt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddMvc().AddRazorPagesOptions(options => options.Conventions.AddPageRoute("/CRUD/Index", ""));
builder.Services.AddDbContext<Itemdb>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ItemDataBase")));
builder.Services.AddDbContext<Userdb>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ItemDataBase")));
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(36000);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapRazorPages();

app.Run();
