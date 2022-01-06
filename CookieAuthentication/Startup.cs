using CookieAuthentication.Authorization;
using CookieAuthentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CookieAuthentication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddAuthentication("cookieAuth").AddCookie("cookieAuth", options =>
            {
                options.Cookie.Name = "cookieAuth";
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/access-denied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
            });

            services.AddAuthorization(configure => {
                configure.AddPolicy("RequireBelongsToHrDept", 
                    configurePolicy => configurePolicy.RequireClaim("department", "HR"));
                configure.AddPolicy("RequireBelongsToHrDeptAdmins",
                    configurePolicy => configurePolicy
                    .RequireClaim(ClaimTypes.Role, "admin")
                    .RequireClaim("department", "HR")
                    .Requirements.Add(new ProbationPassedRequirement(2)));
                });
            services.AddSingleton<IAuthorizationHandler, ProbationPassedRequirmentHandler>();

            // Configure New Http Client 
            services.AddHttpClient(Configuration.GetValue<string>("BookApiHttpClientName"), configureClient =>
            {
                configureClient.BaseAddress = new Uri("https://localhost:44357/");
            });

            // Configure New Http Client 
            services.AddHttpClient(Configuration.GetValue<string>("WeatherForecastApiHttpClientName"), configureClient =>
            {
                configureClient.BaseAddress = new Uri("https://localhost:44331/");
            });

            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
                options.Cookie.HttpOnly = true;
                options.IdleTimeout = TimeSpan.FromHours(8);
            });

            services.AddTransient<ApiService>();
            // Add Concret Implementation for IHttpContextAccessor In order to Inject it.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
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

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
