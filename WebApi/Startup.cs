﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using WebApi.Hubs;
using Domain.Implementation.Services;
using Search.Implementation;
using System.IO;
using Data.Implementation.Repositories;
using Data.Contracts;

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
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUserEntity, RoleEntity>(a =>
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
                a.AddProfile(new AutomapperMessage());
                a.AddProfile(new AutomapperPostProfile());
                a.AddProfile(new AutomapperUserProfile());
                a.AddProfile(new AutomapperTagProfile());
                a.AddProfile(new AutomapperRoleProfile());
                a.AddProfile(new AutomapperClimeProfile());
                a.AddProfile(new AutomapperChatProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();

            services.AddSingleton(mapper);

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
            services.AddScoped<ServiceOfAccount>();
            services.AddScoped<ServiceOfComment>();
            services.AddScoped<ServiceOfImage>();
            services.AddScoped<ServiceOfPost>();
            services.AddScoped<ServiceOfSection>();
            services.AddScoped<ServiceOfTag>();
            services.AddScoped<ServiceOfUser>();
            services.AddScoped<ServiceOfSearch>();
            services.AddScoped<ServiceOfChat>();
            services.AddScoped<ServiceOfMessage>();
            services.AddScoped<IRepository<MessageEntity>, RepositoryOfMessage>();
            services.AddScoped<IRepository<ChatRoomEntity>, RepositoryOfChatRoom>();
            services.AddScoped<IRepository<UserChatEntity>, DefaultRepository<UserChatEntity>>();
            services.AddScoped<IRepository<ApplicationUserEntity>, RepositoryOfApplicationUser>();
            services.AddScoped<IRepository<CommentEntity>, RepositoryOfComment>();
            services.AddScoped<IRepository<CommentLikeEntity>, RepositoryOfCommentLike>();
            services.AddScoped<IRepository<IdentityUserRole<string>>, RepositoryOfIdentityUserRole>();
            services.AddScoped<IRepository<ImageEntity>, RepositoryOfImage>();
            services.AddScoped<IRepository<PostEntity>, RepositoryOfPost>();
            services.AddScoped<IRepository<PostRatingEntity>, RepositoryOfPostRating>();
            services.AddScoped<IRepository<PostTagEntity>, RepositoryOfPostTag>();
            services.AddScoped<IRepository<RoleEntity>, RepositoryOfRole>();
            services.AddScoped<IRepository<SectionEntity>, RepositoryOfSection>();
            services.AddScoped<IRepository<TagEntity>, RepositoryOfTag>();
            services.AddScoped<IRepository<IdentityUserClaim<string>>, RepositoryOfUserClaim>();
            services.AddScoped<IRepository<UserProfileEntity>, RepositoryOfUserProfile>();
            services.AddSignalR();
            services.AddLocalization(a => a.ResourcesPath = "Resources");
            services.AddMvc();
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

            app.UseStaticFiles();
            var supportedCultures = new[]
            {
                new CultureInfo("ru-RU"),
                new CultureInfo("en-US"),
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
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
                builder.AllowAnyMethod();
                builder.AllowCredentials();
                //        .AllowAnyHeader()
                //        .AllowCredentials()
                //        .WithOrigins("http://localhost:51319");
            });
            app.UseSignalR(a => a.MapHub<CommentHub>("/commentHub"));
            app.UseSignalR(a => a.MapHub<MessageHub>("/MessageHub"));
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseBlazor<WebBlazor.Program>();
        }
    }
}
