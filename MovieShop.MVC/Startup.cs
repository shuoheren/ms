using System;
using ApplicationCore.ServiceInterfaces;
using Azure.Storage.Blobs;
using Infrastructure.Data;
using Infrastructure.Helpers;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MovieShop.MVC
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
            services.AddDbContext<MovieShopDbContext>(options =>
                options.UseSqlServer(Configuration
                    .GetConnectionString("MovieShopDbConnection")));
            services.AddAutoMapper(typeof(Startup), typeof(MovieShopMappingProfile));
            ConfigureDependencyInjection(services);
            services.AddMemoryCache();
            services.AddHttpContextAccessor();

            //sets the default authentication scheme for the app
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.Cookie.Name = "MovieShopAuthCookie";
                options.ExpireTimeSpan = TimeSpan.FromHours(2);
                options.LoginPath = "/Account/Login";
            });
        }

        private void ConfigureDependencyInjection(IServiceCollection services)
        {
            var connectionString = Configuration.GetValue<string>("AzureBlobStorage");
           // var containerName = Configuration.GetValue<string>("MovieShopBlobContainer");
            services.AddTransient<IBlobService>(b =>
                new BlobService(new BlobServiceClient(connectionString)));

            services.AddRepositories();
            services.AddServices();
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

            //If the app uses authentication / authorization features such as AuthorizePage or[Authorize], place the call to UseAuthentication and UseAuthorization after UseRouting(and after UseCors if CORS Middleware is used).

            //The trick (which is not mentioned in the migration guide) appears to be that
            //UseAuthenticationUseAuthorization needs to be AFTER UseRouting but BEFORE UseEndpoints.


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}