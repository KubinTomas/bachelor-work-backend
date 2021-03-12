using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using bachelor_work_backend.AutoMapper;
using bachelor_work_backend.Database;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace bachelor_work_backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public const string EnableCorsPolicy = "EnableCORS";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
          
            // UI use PROXY
            // DEPLOYE, pouzit URL a UI pod domenu
            // V ramci rychlosti zahodit cors

            // Mozna auth nahradit COOKIE ...
            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-5.0&fbclid=IwAR2I7Ib6eTE6BlIj6EQ9REYDL5iLrFPoWYrduJODP3gfxpAouyGFiI3RE1s

            // IOPTIONS misto configu
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0&fbclid=IwAR2rQZwHZjHpcuRZIZx555wgaP6DNwENz6y2OF5aL5YnL9ziaGEPP_FzgjQ

            // appsettings dev... pred deplo.. NEDAVAT DO GITU!

            // HttpClient approche
            // https://stackoverflow.com/questions/59280153/dependency-injection-httpclient-or-httpclientfactory?fbclid=IwAR2DMiQrsPyCA1fy64UU0qOtO1EPA09kFc4LVK_aHnvTAFQtZboGi_PAXLg

            var server = Configuration.GetSection("ConnectionStrings").GetValue<string>("BachContext");

            services.AddDbContext<BachContext>(options =>
                options.UseSqlServer(server));

            services.AddCors(opt =>
            {
                //opt.AddPolicy(EnableCorsPolicy, builder =>
                //{
                //    builder.AllowAnyOrigin()
                //    .AllowAnyHeader()
                //    .AllowAnyMethod();
                //});
                opt.AddPolicy(EnableCorsPolicy,
                    builder => builder.WithOrigins("http://localhost:4200")
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

        
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {

                    });

            services.AddControllers();
            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(EnableCorsPolicy);

            var origins = new List<string>()
            {
                "http://localhost:4200",
                "https://shinyapps.ki.ujep.cz/restag"
            };

            app.UseCors(
                options => options.WithOrigins(origins.ToArray()).AllowAnyMethod()
            );

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.None,
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
