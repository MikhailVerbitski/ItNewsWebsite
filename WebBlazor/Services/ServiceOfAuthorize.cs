using Blazor.Extensions.Storage;
using Domain.Contracts.Models;
using Domain.Contracts.Models.ViewModels;
using Domain.Contracts.Models.ViewModels.Account;
using Microsoft.AspNetCore.Blazor;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebBlazor.Services
{
    class TokenWrapper
    {
        public string Token { get; set; }
    }
    public class ServiceOfAuthorize
    {
        private readonly LocalStorage localStorage;
        private readonly HttpClient Http;

        public DataAboutCurrentUser DataAboutUser { get; set; }
        public Task CreateHeader { get; set; }
        public bool IsAuthorize { get; set; } = false;
        public event Action UpdateAfterAuthorization;
        
        public ServiceOfAuthorize(LocalStorage localStorage, HttpClient Http)
        {
            this.localStorage = localStorage;
            this.Http = Http;

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
                UpdateAfterAuthorization?.Invoke();
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
    }
}