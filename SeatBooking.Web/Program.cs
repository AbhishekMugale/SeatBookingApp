using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SeatBooking.DAL.DataContext;
using SeatBooking.DAL.DALRepository;
using SeatBooking.DAL.DALRepository.Contracts;
using SeatBooking.Repository.Business_Services;
using SeatBooking.Repository.Business_Services.Business_Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SeatBooking.Repository;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BookingDatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Sql"));
});

// Add JWT secret from configuration
string jwtSecret = builder.Configuration["JwtSecret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("JWT secret is missing in configuration.");
}
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IUserService>(sp => new UserService(
    sp.GetRequiredService<IUserRepository>(),
    jwtSecret));
builder.Services.AddHttpContextAccessor();
builder.Services.AddHostedService<BookingStatusUpdaterService>();

;

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie(options =>
       {
           options.LoginPath = "/Login/Login";

           options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // Set the login path
       });


var app = builder.Build();

// Configure the HTTP request pipeline.
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
























//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using SeatBooking.DAL.DataContext;
//using SeatBooking.DAL.DALRepository;
//using SeatBooking.DAL.DALRepository.Contracts;
//using SeatBooking.Repository.Business_Services;
//using SeatBooking.Repository.Business_Services.Business_Contracts;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();
//builder.Services.AddDbContext<BookingDatabaseContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("Sql"));
//});

//// Add JWT secret from configuration
//string jwtSecret = builder.Configuration["JwtSecret"];
//if (string.IsNullOrEmpty(jwtSecret))
//{
//    throw new InvalidOperationException("JWT secret is missing in configuration.");
//}

//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
//builder.Services.AddScoped<IEmployeeService, EmployeeService>();
//builder.Services.AddScoped<IUserService>(sp => new UserService(
//    sp.GetRequiredService<IUserRepository>(),
//    jwtSecret));
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//            .AddCookie(options =>
//            {
//                options.LoginPath = "/Login/Login"; // Specify your login path
//                options.LogoutPath = "/Login/Logout"; // Specify your logout path
//            });


//var app = builder.Build();


//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseMiddleware<AuthenticationMiddleware>();
//app.UseRouting();

//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();

