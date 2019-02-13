using Blazor.Extensions.Storage;
using Domain.Contracts.Models;
using Domain.Contracts.Models.ViewModels;
using Domain.Contracts.Models.ViewModels.Account;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebBlazor.Components
{
    class TokenWrapper
    {
        public string Token { get; set; }
    }
    public class ServiceOfAuthorize
    {
        private readonly LocalStorage localStorage;
        private readonly HttpClient Http;
        private readonly IUriHelper UriHelper;

        public DataAboutCurrentUser DataAboutUser { get; set; }
        public Task CreateHeader { get; set; }
        public bool IsAuthorize { get; set; } = false;
        public event Action UpdateAfterAuthorization;

        public ServiceOfAuthorize(LocalStorage localStorage, HttpClient Http, IUriHelper UriHelper)
        {
            this.localStorage = localStorage;
            this.Http = Http;
            this.UriHelper = UriHelper;

            CreateHeader = Login();
        }
        public async Task<TokenViewModel> Login(LoginViewModel loginViewModel = null)
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
                try
                {
                    await UpdateUserData();
                }
                catch
                {
                    await Logout();
                }
                UpdateAfterAuthorization.Invoke();
            }
            return result;
        }
        public async Task UpdateUserData()
        {
            DataAboutUser = await Http.GetJsonAsync<DataAboutCurrentUser>("/api/User/GetDataAboutCurrentUser");
        }
        public async Task Logout()
        {
            await localStorage.RemoveItem("token");
            Http.DefaultRequestHeaders.Remove("Authorization");
            IsAuthorize = false;
            DataAboutUser = null;
            UpdateAfterAuthorization.Invoke();
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
                return (await ProblemWithQuery()) ? null : await GetJsonAsync<T>(requestUri);
            }
        }
        public async Task<T> PostJsonAsync<T>(string requestUri, object content) where T : class
        {
            await CreateHeader;
            try
            {
                return await Http.PostJsonAsync<T>(requestUri,content);
            }
            catch(System.Runtime.Serialization.SerializationException ex)
            {
                return null;
            }
            catch
            {
                return (await ProblemWithQuery()) ? null : await PostJsonAsync<T>(requestUri, content);
            }
        }
        public async Task<T> SendJsonAsync<T>(HttpMethod method, string requestUri, object content) where T : class
        {
            await CreateHeader;
            try
            {
                return await Http.SendJsonAsync<T>(method, requestUri, content);
            }
            catch (System.Runtime.Serialization.SerializationException ex)
            {
                return null;
            }
            catch
            {
                return (await ProblemWithQuery()) ? null : await SendJsonAsync<T>(method, requestUri, content);
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
                return (await ProblemWithQuery()) ? null : await GetAsync(requestUri);
            }
        }
        private async Task<bool> ProblemWithQuery()
        {
            if (Http.DefaultRequestHeaders.Contains("Authorization"))
            {
                await Logout();
                UriHelper.NavigateTo("/Login");
                return false;
            }
            var result = await Login();
            if(result == null)
            {
                UriHelper.NavigateTo("/Login");
            }
            return true;
        }
    }
}