namespace Domain.Contracts.Models
{
    public class ImageViewModel
    {
        public int PostId { get; set; }
        public byte[] Data { get; set; }
        public string Extension { get; set; }
    }
}
