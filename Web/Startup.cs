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

            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
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
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddMvc()
                .AddFluentValidation(a =>
                {
                    a.RegisterValidatorsFromAssemblyContaining<LoginValidator>();
                    a.RegisterValidatorsFromAssemblyContaining<RegisterValidator>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
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
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Images")),
                RequestPath = "/Images"
            });
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=ForTests}/{action=Index}/{id?}");
            });
        }
    }
}
