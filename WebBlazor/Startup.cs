using Blazor.Extensions.Storage;
using Blazor.FileReader;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using WebBlazor.Components;
using WebBlazor.JsInteropClasses;

namespace WebBlazor
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddStorage();
            services.AddSingleton<IFileReaderService>(sp => new FileReaderService());
            services.AddSingleton<ServiceOfAuthorize>();
            services.AddSingleton<ServiceOfLocalization>();
            services.AddScoped<ServiceOfImage>();
            services.AddScoped<ServiceOfTheme>(); 
        }

        public void Configure(IBlazorApplicationBuilder app)
        {
            app.UseLocalTimeZone();
            app.AddComponent<App>("app");
        }
    }
}
