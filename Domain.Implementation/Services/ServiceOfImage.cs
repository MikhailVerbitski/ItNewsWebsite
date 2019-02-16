using Data.Implementation;
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

        public ServiceOfImage(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            var folders = hostingEnvironment.ContentRootPath.Split('\\');
            this.SolutionPath = string.Join('\\', folders.Take(folders.Length - 1));
            this.SolutionPath += @"\WebBlazor";

            repositoryOfImage = new RepositoryOfImage(context);
        }

        public string LoadImage(string host, string applicationUserCurrent, UserImage image) => LoadImage(host, "Avatars", applicationUserCurrent, image.Extension, image.Data, true);
        public string LoadImage(string host, string applicationUserCurrent, PostImage image) => LoadImage(host, "Post", applicationUserCurrent, image.Extension, image.Data, false);
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

        //public string RenameImage(string folder, string lastPath, string newName)
        //{
        //    var filePath = $"{string.Join('/', SolutionPath.Split("\\"))}/{lastPath}";
        //    var mas = lastPath.Split('/');
        //    var extension = mas.Last().Split('.').Last();
        //    var newPath = $"{string.Join('/', mas.Take(mas.Length - 1).ToArray())}/{newName}";
        //    int addition = 0;
        //    while(File.Exists($"{newPath}_{addition}.{extension}"))
        //    {
        //        addition++;
        //    }
        //    newPath = $"{newPath}_{addition}.{extension}";
        //    File.Move(filePath, string.Join('/', SolutionPath.Split("\\")) + newPath);
        //    return newPath;
        //}

        public void Delete(string path)
        {
            if(File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
