using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Blazor.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Data.Implementation;
using Data.Contracts.Models.Entities;
using Infrastructure.AutomapperProfiles;
using System.Linq;
using System.Net.Mime;
using System.Text;
using WebApi.Server.Service;
using WebApi.Server.Interface;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace WebApi
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
            services.AddLocalization(option => option.ResourcesPath = "Resources");

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            
            services.AddIdentity<ApplicationUserEntity, RoleEntity>(a =>
            {
                a.Password.RequireNonAlphanumeric = false;
                a.Password.RequireLowercase = false;
                a.Password.RequireUppercase = false;
                a.Password.RequireDigit = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            var mappingConfig = new MapperConfiguration(a =>
            {
                a.AddProfile(new AutomapperCommentProfile());
                a.AddProfile(new AutomapperPostProfile());
                a.AddProfile(new AutomapperUserProfile());
                a.AddProfile(new AutomapperTagProfile());
                a.AddProfile(new AutomapperRoleProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();

            services.AddSingleton(mapper);
            services.AddCors();
            services.AddMvc();

            services.AddTransient<IJwtTokenService, JwtTokenService>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                    };
                });
            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm,
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment hostingEnvironment)
        {
            app.UseResponseCompression();
            if (hostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();
            }


            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("ru"),
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });


            var folders = hostingEnvironment.ContentRootPath.Split('\\');
            var SolutionPath = string.Join('\\', folders.Take(folders.Length - 1));
            SolutionPath += @"\WebBlazor";
            SolutionPath += @"\Images";
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(SolutionPath),
                RequestPath = "/Images"
            });
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyHeader();
            });
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseBlazor<WebBlazor.Program>();
        }
    }
}
