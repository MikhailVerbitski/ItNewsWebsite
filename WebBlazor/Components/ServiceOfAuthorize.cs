using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Blazor.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebBlazor.Components
{
    public class ServiceOfAuthorize
    {
        private readonly LocalStorage localStorage;
        private readonly HttpClient Http;
        private readonly IUriHelper UriHelper;
        
        public Task<bool> Authorize { get; set; }

        public ServiceOfAuthorize(LocalStorage localStorage, HttpClient Http, IUriHelper UriHelper)
        {
            this.localStorage = localStorage;
            this.Http = Http;
            this.UriHelper = UriHelper;
        }
        public async Task<bool> AddTokenInHeader()
        {
            var response = await Http.GetAsync("/api/Token/TokenVerification");
            if (!response.IsSuccessStatusCode)
            {
                if (!Http.DefaultRequestHeaders.Contains("Authorization"))
                {
                    var token = await localStorage.GetItem<string>("token");
                    if (token == null)
                    {
                        UriHelper.NavigateTo("/Login");
                        return false;
                    }
                    Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    await AddTokenInHeader();
                }
                else
                {
                    UriHelper.NavigateTo("/Login");
                    return false;
                }
            }
            return true;
        }
        public Task<bool> TryToAuthorize()
        {
            return Authorize = Task.Run(async () => await AddTokenInHeader());
        }
    }
}
