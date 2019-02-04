using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Services;
using System.Net.Http;
using System.Threading.Tasks;
using WebBlazor.Models.ViewModels;

namespace WebBlazor.Components
{
    class Token
    {
        public string token { get; set; }
    }
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
        private async Task<bool> AddTokenInHeader()
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
            Authorize = Task.Run(async () => await AddTokenInHeader());
            //Task.Run(async () => { authorize = await Authorize; System.Console.WriteLine(authorize); });
            return Authorize;
        }
        public async Task<TokenViewModel> Login(WebBlazor.Models.ViewModels.Account.LoginViewModel loginViewModel)
        {
            var result = await Http.PostJsonAsync<TokenViewModel>("/api/Token/Login", loginViewModel);
            await localStorage.SetItem<string>("token", result.Token);
            await TryToAuthorize();
            return result;
        }
        public void Logout()
        {
            localStorage.RemoveItem("token");
            Http.DefaultRequestHeaders.Remove("Authorization");
        }
        public async Task<T> GetJsonAsync<T>(string requestUri) where T: class
        {
            if (await Authorize)
            {
                try
                {
                    return await Http.GetJsonAsync<T>(requestUri);
                }
                catch
                {
                    await TryToAuthorize();
                }
            }
            UriHelper.NavigateTo("/Login");
            return null;
        }
        public async Task<T> PostJsonAsync<T>(string requestUri, object content) where T : class
        {
            if (await Authorize)
            {
                try
                {
                    return await Http.PostJsonAsync<T>(requestUri,content);
                }
                catch
                {
                    await TryToAuthorize();
                }
            }
            UriHelper.NavigateTo("/Login");
            return null;
        }
        public async Task<T> SendJsonAsync<T>(HttpMethod method, string requestUri, object content) where T : class
        {
            if (await Authorize)
            {
                try
                {
                    return await Http.SendJsonAsync<T>(method, requestUri, content);
                }
                catch
                {
                    await TryToAuthorize();
                }
            }
            UriHelper.NavigateTo("/Login");
            return null;
        }
        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            if (await Authorize)
            {
                try
                {
                    return await Http.GetAsync(requestUri);
                }
                catch
                {
                    await TryToAuthorize();
                }
            }
            UriHelper.NavigateTo("/Login");
            return null;
        }
    }
}