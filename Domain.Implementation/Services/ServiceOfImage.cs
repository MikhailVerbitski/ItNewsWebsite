using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;

namespace Domain.Implementation.Services
{
    public class ServiceOfImage
    {
        private readonly string SolutionPath;

        private readonly RepositoryOfImage repositoryOfImage;

        public ServiceOfImage(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            var folders = hostingEnvironment.ContentRootPath.Split('\\');
            this.SolutionPath = string.Join('\\', folders.Take(folders.Length - 1));
            this.SolutionPath += @"\WebBlazor";

            repositoryOfImage = new RepositoryOfImage(context);
        }

        public string LoadImage(string folder, ImageViewModel image) => LoadImage(folder, image.PostId.ToString(), image.Extension, image.Data);
        public string LoadImage(string folder, string name, string extension, byte[] date, bool isRewrite = false)
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
            string result = $"/Images/{folder}/{fileName}";
            return result;
        }
        public void Delete(string path)
        {
            if(File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
