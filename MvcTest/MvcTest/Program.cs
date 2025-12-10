using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using MyAccounting.Data;
using MyAccounting.Middleware;

namespace MvcTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<SqlDBContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    options.SlidingExpiration = true;
                    options.Cookie.MaxAge = TimeSpan.FromDays(7);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.Cookie.Name = ".MyAccounting.Auth";
                    options.LoginPath = "/Users/Login";
                    options.LogoutPath = "/Users/Logout";
                    options.AccessDeniedPath = "/Home/Index";
                });

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.Name = ".MyAccounting.Session";
                options.Cookie.IsEssential = true;
            });

            // Swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MyAccounting API",
                    Version = "v1",
                    Description = "MyAccounting API Documentation",
                    Contact = new OpenApiContact
                    {
                        Name = "Hamed Movaghari",
                        Email = "hmovaghari@gmail.com"
                    }
                });

                //// Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
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

            app.UseRouting();

            // فعال‌سازی Session - باید قبل از Authentication باشد
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            // API Key Validation Middleware - قبل از Swagger
            app.UseApiKeyValidation();

            // Swagger با محافظت API Key
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwaggerAuth();
            app.UseSwagger();
            app.UseSwaggerUI();
            //}

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
