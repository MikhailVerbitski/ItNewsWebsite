using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

        public string LoadImage(string folder, string name, IFormFile image, bool isRewrite = false)
        {
            var extension = Path.GetExtension(image.FileName);
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
                image.CopyToAsync(stream).Wait();
            }
            string result = $"/Images/{folder}/{fileName}";
            return result;
        }

        public ImageEntity CreateImageForPost(int postId, IFormFile image)
        {
            string path = LoadImage("Post", postId.ToString(), image);
            var imageEntity = repositoryOfImage.Create(new ImageEntity
            {
                Path = path,
                PostId = postId
            });
            return imageEntity;
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
