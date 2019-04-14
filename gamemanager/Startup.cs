using gamemanager.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace gamemanager
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            /*---MDB This section is needed to enable session---*/
            services.AddDistributedMemoryCache();
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(1);
            });
            /*--------------------------------------------------*/

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //This line has been added so that the application registers the db context for use - it gets its connection string from appsettings.json
            Code.AppSettingManager appSettingManager = Code.AppSettingManager.Instance;
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            connectionString = connectionString
                .Replace("__host__", appSettingManager.GetSetting("host"))
                .Replace("__username__", appSettingManager.GetSetting("username"))
                .Replace("__password__", appSettingManager.GetSetting("password"));

            services.Add(new ServiceDescriptor(typeof(DataContext), new DataContext(connectionString)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            /*---MDB This section is needed to enable session---*/
            app.UseSession();
            /*--------------------------------------------------*/

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
