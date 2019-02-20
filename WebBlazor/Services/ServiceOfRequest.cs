using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Services;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebBlazor.Services
{
    public class ServiceOfRequest
    {
        private readonly HttpClient Http;
        private readonly IUriHelper UriHelper;
        private readonly ServiceOfAuthorize serviceOfAuthorize;

        public ServiceOfRequest(HttpClient Http, IUriHelper UriHelper, ServiceOfAuthorize serviceOfAuthorize)
        {
            this.Http = Http;
            this.UriHelper = UriHelper;
            this.serviceOfAuthorize = serviceOfAuthorize;
        }
        public async Task<T> GetJsonAsync<T>(string requestUri) where T : class
        {
            await serviceOfAuthorize.CreateHeader;
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
            await serviceOfAuthorize.CreateHeader;
            try
            {
                return await Http.PostJsonAsync<T>(requestUri, content);
            }
            catch (System.Runtime.Serialization.SerializationException)
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
            await serviceOfAuthorize.CreateHeader;
            try
            {
                return await Http.SendJsonAsync<T>(method, requestUri, content);
            }
            catch (System.Runtime.Serialization.SerializationException)
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
            await serviceOfAuthorize.CreateHeader;
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
                await serviceOfAuthorize.Logout();
                UriHelper.NavigateTo("/Login");
                return false;
            }
            var result = await serviceOfAuthorize.Login();
            if (result == null)
            {
                UriHelper.NavigateTo("/Login");
            }
            return true;
        }
    }
}
