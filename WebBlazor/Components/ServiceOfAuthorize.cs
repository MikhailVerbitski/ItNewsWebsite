using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Services;
using System;
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

        public bool IsAuthorize { get; set; } = false;
        public event Action UpdateNavbar;
        private Task CreateHeader;

        public ServiceOfAuthorize(LocalStorage localStorage, HttpClient Http, IUriHelper UriHelper)
        {
            this.localStorage = localStorage;
            this.Http = Http;
            this.UriHelper = UriHelper;

            CreateHeader = CheckAuthorization();
        }
        public async Task<TokenViewModel> Login(WebBlazor.Models.ViewModels.Account.LoginViewModel loginViewModel)
        {
            var result = await Http.PostJsonAsync<TokenViewModel>("/api/Token/Login", loginViewModel);
            await localStorage.SetItem<string>("token", result.Token);
            IsAuthorize = true;
            UpdateNavbar.Invoke();
            return result;
        }
        public async Task Logout()
        {
            await localStorage.RemoveItem("token");
            Http.DefaultRequestHeaders.Remove("Authorization");
            IsAuthorize = false;
            UpdateNavbar.Invoke();
            Console.WriteLine("logout");
        }
        private async Task CheckAuthorization()
        {
            var token = await localStorage.GetItem<string>("token");
            if (token == null)
            {
                IsAuthorize = false;
            }
            Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await Http.GetAsync("/api/Token/TokenVerification");
            IsAuthorize = response.IsSuccessStatusCode;
            UpdateNavbar.Invoke();
            return;
        }
        public async Task<T> GetJsonAsync<T>(string requestUri) where T: class
        {
            await CreateHeader;
            try
            {
                return await Http.GetJsonAsync<T>(requestUri);
            }
            catch
            {
                if (Http.DefaultRequestHeaders.Contains("Authorization"))
                {
                    await Logout();
                    UriHelper.NavigateTo("/Login");
                    return null;
                }
                else
                {
                    var token = await localStorage.GetItem<string>("token");
                    if (token == null)
                    {
                        UriHelper.NavigateTo("/Login");
                        return null;
                    }
                    Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    return await GetJsonAsync<T>(requestUri);
                }
            }
        }
        public async Task<T> PostJsonAsync<T>(string requestUri, object content) where T : class
        {
            await CreateHeader;
            try
            {
                return await Http.PostJsonAsync<T>(requestUri,content);
            }
            catch
            {
                if (Http.DefaultRequestHeaders.Contains("Authorization"))
                {
                    await Logout();
                    UriHelper.NavigateTo("/Login");
                    return null;
                }
                else
                {
                    var token = await localStorage.GetItem<string>("token");
                    if (token == null)
                    {
                        UriHelper.NavigateTo("/Login");
                        return null;
                    }
                    Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    return await PostJsonAsync<T>(requestUri, content);
                }
            }
        }
        public async Task<T> SendJsonAsync<T>(HttpMethod method, string requestUri, object content) where T : class
        {
            await CreateHeader;
            try
            {
                return await Http.SendJsonAsync<T>(method, requestUri, content);
            }
            catch
            {
                if (Http.DefaultRequestHeaders.Contains("Authorization"))
                {
                    await Logout();
                    UriHelper.NavigateTo("/Login");
                    return null;
                }
                else
                {
                    var token = await localStorage.GetItem<string>("token");
                    if (token == null)
                    {
                        UriHelper.NavigateTo("/Login");
                        return null;
                    }
                    Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    return await SendJsonAsync<T>(method, requestUri, content);
                }
            }
        }
        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            await CreateHeader;
            try
            {
                return await Http.GetAsync(requestUri);
            }
            catch
            {
                if (Http.DefaultRequestHeaders.Contains("Authorization"))
                {
                    await Logout();
                    UriHelper.NavigateTo("/Login");
                    return null;
                }
                else
                {
                    var token = await localStorage.GetItem<string>("token");
                    if (token == null)
                    {
                        UriHelper.NavigateTo("/Login");
                        return null;
                    }
                    Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    return await GetAsync(requestUri);
                }
            }
        }
    }
}