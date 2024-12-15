using ChatSignalR.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ChatSignalR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSignalR();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Home/Register";
                options.AccessDeniedPath = "/Home/AccessDenied";
            });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession();

            builder.Services.AddControllersWithViews();
            var app = builder.Build();

            app.UseStaticFiles();

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapHub<ChatHub>("/chat");
            app.Run();
        }
    }
}
