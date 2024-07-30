using DigitBlog.Models;
using DigitBlog.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace DigitBlog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<DataSecurityKey>();
            builder.Services.AddDbContext<DigitalBlogContext>(o => o.UseSqlServer(builder.Configuration["Conn"]));
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o=>o.LoginPath="/Account/Login");
            builder.Services.AddSession(o =>
            {
                o.IdleTimeout = TimeSpan.FromMinutes(2);
                o.Cookie.HttpOnly=true;
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Static}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
