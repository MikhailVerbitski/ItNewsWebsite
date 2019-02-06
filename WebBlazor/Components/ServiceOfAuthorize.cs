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

        public string UserLogin { get; set; }
        private Task CreateHeader { get; set; }
        public bool IsAuthorize { get; set; } = false;
        public event Action UpdateAfterAuthorization;

        public ServiceOfAuthorize(LocalStorage localStorage, HttpClient Http, IUriHelper UriHelper)
        {
            this.localStorage = localStorage;
            this.Http = Http;
            this.UriHelper = UriHelper;

            CreateHeader = Login();
        }
        public async Task<TokenViewModel> Login(WebBlazor.Models.ViewModels.Account.LoginViewModel loginViewModel = null)
        {
            string token;
            TokenViewModel result = null;
            if(loginViewModel == null)
            {
                token = await localStorage.GetItem<string>("token");
            }
            else
            {
                result = await Http.PostJsonAsync<TokenViewModel>("/api/Token/Login", loginViewModel);
                token = result.Token;
                await localStorage.SetItem<string>("token", token);
            }
            IsAuthorize = token != null;
            if(IsAuthorize)
            {
                Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                UserLogin = await Http.GetJsonAsync<string>("/api/User/GetUserLogin");
                UpdateAfterAuthorization.Invoke();
            }
            return result;
        }
        public async Task Logout()
        {
            await localStorage.RemoveItem("token");
            Http.DefaultRequestHeaders.Remove("Authorization");
            IsAuthorize = false;
            UpdateAfterAuthorization.Invoke();
        }
        public async Task<T> GetJsonAsync<T>(string requestUri) where T: class
        {
            try
            {
                return await Http.GetJsonAsync<T>(requestUri);
            }
            catch
            {
                return (await ProblemWithQuery()) ? null : await GetJsonAsync<T>(requestUri);
            }
        }
        public async Task<T> PostJsonAsync<T>(string requestUri, object content) where T : class
        {
            try
            {
                return await Http.PostJsonAsync<T>(requestUri,content);
            }
            catch
            {
                return (await ProblemWithQuery()) ? null : await PostJsonAsync<T>(requestUri, content);
            }
        }
        public async Task<T> SendJsonAsync<T>(HttpMethod method, string requestUri, object content) where T : class
        {
            try
            {
                return await Http.SendJsonAsync<T>(method, requestUri, content);
            }
            catch
            {
                return (await ProblemWithQuery()) ? null : await SendJsonAsync<T>(method, requestUri, content);
            }
        }
        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            try
            {
                return await Http.GetAsync(requestUri);
            }
            catch
            {
                return (await ProblemWithQuery()) ? null : await GetAsync(requestUri);
            }
        }
        private async Task<bool> ProblemWithQuery()
        {
            if (Http.DefaultRequestHeaders.Contains("Authorization"))
            {
                await Logout();
                UriHelper.NavigateTo("/Login");
                return true;
            }
            await Login();
            return false;
        }
    }
}