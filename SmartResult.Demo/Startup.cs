using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AspNet.Core.SmartResult.Demo.Mappings;
using SmartResult.Demo;

namespace AspNet.Core.SmartResult.Demo
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
            services.AddSingleton<IRepository, Repository>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add a list of AutoMapper profiles to be used by SmartResult
            List<Profile> profiles = new List<Profile> { new SmartResultProfile() };

            // Use the minimum configuration
            SmartResult.Configure(
                new SmartResultConfiguration(
                    profiles,
                    isMobile: MyCustomMobileDetection,
                    isNative: MyCustomNativeDetection
                )
            );

            app.UseMvc();
        }

        private bool MyCustomMobileDetection(HttpContext request)
        {
            // Place your custom logic here for detecting Mobile browsers
            return true;
        }

        private bool MyCustomNativeDetection(HttpContext request)
        {
            // Place your custom logic here for detecting Native devices
            return true;
        }
    }
}
