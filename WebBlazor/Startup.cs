using Blazor.Extensions.Storage;
using Blazor.FileReader;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection; 

namespace WebBlazor
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddStorage();
            services.AddSingleton<IFileReaderService>(sp => new FileReaderService());
            services.AddScoped(typeof(Components.ServiceOfAuthorize), typeof(Components.ServiceOfAuthorize));
        }

        public void Configure(IBlazorApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
