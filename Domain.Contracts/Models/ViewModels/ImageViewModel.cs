namespace Domain.Contracts.Models.ViewModels
{
    public class ImageViewModel
    {
        public int PostId { get; set; }
        public byte[] Data { get; set; }
        public string Extension { get; set; }
    }
}
