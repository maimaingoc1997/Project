using System.Security.Claims;
using CourseShopOnline.DataAccess.Context;
using CourseShopOnline.Models;
using CourseShopOnline.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Cấu hình Authentication và Authorization
// builder.Services.AddAuthentication()
//     .AddCookie(options =>
//     {
//         options.LoginPath = "/Account/Login";
//         options.AccessDeniedPath = "/Account/AccessDenied";
//         options.LogoutPath = "/Account/Logout";
//     });
// builder.Services.AddDistributedMemoryCache();  // Dịch vụ lưu trữ bộ nhớ cho Session
// builder.Services.AddSession(options =>
// {
//     options.IdleTimeout = TimeSpan.FromMinutes(30);  // Thời gian hết hạn session
//     options.Cookie.HttpOnly = true;  // Chỉ truy cập qua HTTP, không thể truy cập qua JavaScript
//     options.Cookie.IsEssential = true;  // Cần thiết cho cookie trong ứng dụng của bạn
// });
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";  // Đường dẫn cho đăng nhập
        options.LogoutPath = "/Account/Logout";  // Đường dẫn cho đăng xuất
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(1);  // Thời gian sống của cookie
    });

// Cấu hình Session nếu bạn sử dụng session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1);  // Thời gian hết hạn của session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TeacherPolicy", policy =>
        policy.RequireRole("Teacher"));
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Gọi phương thức Initialize để tạo các role và người dùng Admin nếu chưa có
    await SeedRoles.Initialize(services, userManager, roleManager);
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession(); 
app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();