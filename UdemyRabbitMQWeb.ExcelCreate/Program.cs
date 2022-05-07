using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using UdemyRabbitMQWeb.ExcelCreate.Hubs;
using UdemyRabbitMQWeb.ExcelCreate.Models;
using UdemyRabbitMQWeb.ExcelCreate.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton(sp => new ConnectionFactory()
{
    Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")),
    DispatchConsumersAsync = true
});

builder.Services.AddSingleton<RabbitMQClientService>();

builder.Services.AddSingleton<RabbitMQPublisher>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

var app = builder.Build();

//işlem bittikten sonra eldeki servisleri memory den düşürmek için.
using (var scope = app.Services.CreateScope())
{
    //for auto migration
    var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    //for user Create if user  any
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    //uygulama ayağa kalkarken Migrations db ye işler
    appDbContext.Database.Migrate();

    if (!appDbContext.Users.Any())
    {
        userManager.CreateAsync(new IdentityUser() { UserName = "deneme", Email = "deneme@outlook.com" }, "Password12*").Wait();
        userManager.CreateAsync(new IdentityUser() { UserName = "deneme2", Email = "deneme2@outlook.com" }, "Password12*").Wait();
    }
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

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<MyHub>("/MyHub");
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
