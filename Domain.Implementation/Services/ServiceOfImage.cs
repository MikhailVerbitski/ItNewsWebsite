using Data.Implementation.Repositories;
using Domain.Contracts.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;

namespace Domain.Implementation.Services
{
    public class ServiceOfImage
    {
        private readonly string SolutionPath;
        private readonly RepositoryOfImage repositoryOfImage;

        public ServiceOfImage(IHostingEnvironment hostingEnvironment, RepositoryOfImage repositoryOfImage)
        {
            this.repositoryOfImage = repositoryOfImage;
            var folders = hostingEnvironment.ContentRootPath.Split('\\');
            this.SolutionPath = string.Join('\\', folders.Take(folders.Length - 1));
            this.SolutionPath += @"\WebBlazor";
        }

        public string LoadImage(string host, string applicationUserCurrent, UserImage image)
        {
            var data = System.Convert.FromBase64String(image.Data);
            return LoadImage(host, "Avatars", applicationUserCurrent, image.Extension, data, true);
        }
        public string LoadImage(string host, string applicationUserCurrent, PostImage image)
        {
            var data = System.Convert.FromBase64String(image.Data);
            return LoadImage(host, "Post", applicationUserCurrent, image.Extension, data, false);
        }
        public string LoadImage(string host, string folder, string name, string extension, byte[] date, bool isRewrite = false)
        {
            int addition = 0;
            while(!isRewrite && File.Exists(Path.Combine(SolutionPath, "Images", folder, $"{name}_{addition}{extension}")))
            {
                addition++;
            }
            var fileName = isRewrite ? $"{name}{extension}" : $"{name}_{addition}{extension}";
            var filePath = Path.Combine(SolutionPath, "Images", folder, fileName);
            if(!Directory.Exists(Path.Combine(SolutionPath, "Images", folder)))
            {
                Directory.CreateDirectory(Path.Combine(SolutionPath, "Images", folder));
            }
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                new MemoryStream(date).CopyToAsync(stream).Wait();
            }
            string result = $"{host}/Images/{folder}/{fileName}";
            return result;
        }
        public void Delete(string path)
        {
            var mas = path.Split('/').ToList();
            path = SolutionPath + "\\" + mas.Skip(mas.IndexOf("Images")).Aggregate((a, b) => a + "\\" + b);
            if(File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
