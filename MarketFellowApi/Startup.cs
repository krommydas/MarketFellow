using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FellowLibrary.DatabaseAccess;
using Microsoft.Extensions.Options;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System;

namespace MarketFellowApi
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.Configure<FellowLibrary.Crawler.MarketProvidersConfiguration>(Configuration.GetSection("MarketProviders"));
            services.Configure<DatabaseSettings>(Configuration.GetSection(nameof(DatabaseSettings)));

            services.AddSingleton<IDatabaseSettings>(sp => sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<FellowLibrary.Crawler.MarketProvidersConfiguration>>().Value);

            services.AddScoped<IDependencyResolver>(x => new FuncDependencyResolver(x.GetRequiredService));
            services.AddScoped<Models.MarketFellowSchema>();
            services.AddGraphQL(x => { x.ExposeExceptions = true; x.EnableMetrics = true; })
                .AddGraphTypes(ServiceLifetime.Scoped).AddWebSockets();
           // services.AddSingleton<GraphQLHttpMiddleware>();


            services.AddTransient<DatabaseContext>();
            //services.AddTransient<ClientWebSocket>();

            services.AddHttpClient<Models.MarketFellowQuery>();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault | SecurityProtocolType.Tls;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
          
            //app.UseCors(x => x.WithOrigins)
            app.UseCors(policy => { policy.AllowAnyOrigin(); policy.AllowAnyHeader(); policy.AllowCredentials(); policy.AllowAnyMethod(); });
            app.UseWebSockets();
            app.UseGraphQL<Models.MarketFellowSchema>();
            app.UseGraphQLWebSockets<Models.MarketFellowSchema>();
          
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());     

            //    app.UseSpa(spa =>
            //    {
            //        // To learn more about options for serving an Angular SPA from ASP.NET Core,
            //        // see https://go.microsoft.com/fwlink/?linkid=864501

            //        spa.Options.SourcePath = "ClientApp";

            //        //if (env.IsDevelopment())
            //        //{
            //        //    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
            //        //}
            //    });
        }
    }
}
