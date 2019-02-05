using Blazor.FileReader;
using Microsoft.AspNetCore.Blazor;
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

            var images = await fileReadService.CreateReference(image).EnumerateFilesAsync();
            var memoryStream = await images.First().CreateMemoryStreamAsync();

            var userImage = new UserImage()
            {
                Data = memoryStream.ToArray(),
                Extension = Extension
            };
            return await Http.SendJsonAsync<string>(HttpMethod.Post, "/api/User/ChangeImage", userImage);
        }
        public async Task<string> LoadPostImage(ElementRef image, int postId)
        {
            var Extension = ".png";

            var images = await fileReadService.CreateReference(image).EnumerateFilesAsync();
            var memoryStream = await images.First().CreateMemoryStreamAsync();

            var postImage = new PostImage()
            {
                PostId = postId,
                Data = memoryStream.ToArray(),
                Extension = Extension
            };
            return await Http.SendJsonAsync<string>(HttpMethod.Post, "/api/Post/AddImage", postImage);
        }
    }
    class UserImage
    {
        public byte[] Data { get; set; }
        public string Extension { get; set; }
    }
    class PostImage
    {
        public int PostId { get; set; }
        public byte[] Data { get; set; }
        public string Extension { get; set; }
    }
}
