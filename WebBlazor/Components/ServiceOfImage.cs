using Blazor.FileReader;
using Domain.Contracts.Models;
using Microsoft.AspNetCore.Blazor;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebBlazor.Components
{
    public class ServiceOfImage
    {
        private readonly HttpClient Http;
        private readonly IFileReaderService fileReadService;

        public ServiceOfImage(HttpClient Http, IFileReaderService fileReadService)
        {
            this.Http = Http;
            this.fileReadService = fileReadService;
        }
        public async Task<string> LoadUserImage(ElementRef image)
        {
            var Extension = ".png";

            var userImage = new UserImage()
            {
                Data = await LoadImage(image),
                Extension = Extension
            };
            return await Http.SendJsonAsync<string>(HttpMethod.Post, "/api/User/ChangeImage", userImage);
        }
        public async Task<string> LoadPostImage(ElementRef image, int postId)
        {
            var Extension = ".png";

            var postImage = new PostImage()
            {
                PostId = postId,
                Data = await LoadImage(image),
                Extension = Extension
            };
            return await Http.SendJsonAsync<string>(HttpMethod.Post, "/api/Post/AddImage", postImage);
        }
        public async Task<string> LoadPostImage(string data, int postId, string Extension)
        {
            var postImage = new PostImage()
            {
                PostId = postId,
                Data = data,
                Extension = Extension
            };
            return await Http.SendJsonAsync<string>(HttpMethod.Post, "/api/Post/AddImage", postImage);
        }
        public async Task<string> LoadImage(ElementRef image)
        {
            var images = await fileReadService.CreateReference(image).EnumerateFilesAsync();
            var data = (await images.First().CreateMemoryStreamAsync()).ToArray();
            return Convert.ToBase64String(data);
        }
    }
}
