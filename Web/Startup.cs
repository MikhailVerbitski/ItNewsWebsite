using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Data.Implementation;
using Data.Contracts.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Infrastructure.AutomapperProfiles;
using Microsoft.Extensions.FileProviders;
using System.IO;
using FluentValidation.AspNetCore;
using Domain.Contracts.Validators.ViewModels.Account;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //services.AddAuthentication(CookieAuthenticationDefaults.)
            //.AddCookie(a =>
            //{
            //    a.LoginPath = new PathString("/Account/Login");
            //    a.AccessDeniedPath = new PathString("/Account/Login");
            //});

            services.AddIdentity<ApplicationUserEntity, IdentityRole>(a =>
            {
                a.Password.RequireNonAlphanumeric = false;
                a.Password.RequireLowercase = false;
                a.Password.RequireUppercase = false;
                a.Password.RequireDigit = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            var mappingConfig = new MapperConfiguration(a =>
            {
                a.AddProfile(new AutomapperCommentProfile());
                a.AddProfile(new AutomapperPostProfile());
                a.AddProfile(new AutomapperUserProfile());
                a.AddProfile(new AutomapperTagProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddScoped<IMapper>(p => mapper);

            services.AddCors();

            services.AddMvc()
                .AddFluentValidation(a =>
                {
                    a.RegisterValidatorsFromAssemblyContaining<LoginValidator>();
                    a.RegisterValidatorsFromAssemblyContaining<RegisterValidator>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseCookiePolicy();
            app.UseStaticFiles();

            var folders = hostingEnvironment.ContentRootPath.Split('\\');
            var SolutionPath = string.Join('\\', folders.Take(folders.Length - 1));
            SolutionPath += @"\WebBlazor";
            SolutionPath += @"\Images";
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(SolutionPath),
                RequestPath = "/Images"
            });
            app.UseAuthentication();
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyHeader();
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=ForTests}/{action=Index}/{id?}");
            });
        }
    }
}
