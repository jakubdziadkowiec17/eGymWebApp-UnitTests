using eGym.Models;
using eGym.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGymService, GymService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IMyTicketService, MyTicketService>();
builder.Services.AddScoped<IOpinionService, OpinionService>();
builder.Services.AddScoped<IAdService, AdService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IClassesService, ClassesService>();
builder.Services.AddScoped<IClassesUserService, ClassesUserService>();
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Str")));
builder.Services.AddIdentity<UserModel, IdentityRole>()
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
