using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EmployeeManagement
{
    public class Startup
    {
        private IConfiguration _config;

        // inject IConfiguration service
        // so that we can access content of appsettings.json
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        // this is also called dependency injection container
        public void ConfigureServices(IServiceCollection services)
        {

            // register DbContext class for EF core.
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));

            // register Identity core for authentication and authorization
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

            // change the password configuration
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 3;
            });


            // add mvc service
            services.AddMvc(config =>
            {
                // set global authorization for all controllers
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));

                // fix for route issue
                config.EnableEndpointRouting = false;
            });


            // add claim-based authorization
            services.AddAuthorization(options =>
                options.AddPolicy("DeleteRolePolicy",
                    policy => policy.RequireClaim("Delete Role")));

            services.AddAuthorization(options =>
                options.AddPolicy("EditRolePolicy",
                    policy => policy.RequireClaim("Edit Role")));

            // add mvc service
            // services.AddMvc(option => option.EnableEndpointRouting = false);

            // return xml format 
            // use it with ObjectResult in controller
            // services.AddMvc(option => option.EnableEndpointRouting = false).AddXmlSerializerFormatters();

            // when  HomeController/Index request IEmployeeRepository
            // then create an instance of MockEmployeeRepository class and inject to the controller
            // services.AddSingleton<IEmployeeRepository, MockEmployeeRepository>();

            // AddScoped: we want the instance to be alive for a given http request
            // and new instance for different/new http request.
            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
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
                // use this middleware to handle exceptions from action methods
                // redirect user to the /Error page
                app.UseExceptionHandler("/Error");

                // use this middleware to handle errors from 400 to 500
                // {0} will be the action method such as 404
                // app.UseStatusCodePagesWithRedirects("/Error/{0}");
                // or use this (perfered, better)
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            // app.UseRouting();

            // app.UseEndpoints(endpoints =>
            // {
            //     endpoints.MapGet("/", async context =>
            //     {
            //         // print current process name that runs the app
            //         // await context.Response.WriteAsync(System.Diagnostics.Process.GetCurrentProcess().ProcessName);

            //         // read content of appstting.json file
            //         await context.Response.WriteAsync(_config["ConnectionString"]);
            //         // await context.Response.WriteAsync(_config["MyKey"]);
            //     });
            // });

            // this middleware must be placed before UseStaticFiles 
            // in order to make it work.
            // app.UseDefaultFiles();

            // add middleware to serve static files
            app.UseStaticFiles();

            // add middleware for authentication
            app.UseAuthentication();

            // add mvc middleware
            // https://localhost:5001/   home will print message from HomeController Index method
            // app.UseMvcWithDefaultRoute();

            // -> Conventional Routing
            // build custom routing rule
            // use ? to make the parameter optional
            // use = to make default home/index/optional_id
            // app.UseMvc(routes =>
            // {
            //     routes.MapRoute("default", "{controller=home}/{action=index}/{id?}");
            // });

            // -> Attribute Routing
            // use this along with attribute in action method
            app.UseMvc();

            // app.Run(async (context) =>
            // {
            //     // await context.Response.WriteAsync(_config["ConnectionString"]);
            //     // await context.Response.WriteAsync(env.EnvironmentName);
            //     // await context.Response.WriteAsync(env.IsDevelopment().ToString());
            //     await context.Response.WriteAsync("hello from terminate middleware");
            // });
        }
    }
}
