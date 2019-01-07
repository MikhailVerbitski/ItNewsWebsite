using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Domain.Implementation.Services
{
    public class ServiceOfImage
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public ServiceOfImage(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
        public string Create(string folder, string name, IFormFile image)
        {
            var extension = Path.GetExtension(image.FileName);

            int addition = 0;
            while(File.Exists($"{name}_{addition}{extension}"))
            {
                addition++;
            }

            var fileName = $"{name}_{addition}{extension}";
            var filePath = Path.Combine(hostingEnvironment.ContentRootPath, "Images", folder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyToAsync(stream).Wait();
            }
            string result = $"/Images/{fileName}";
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
