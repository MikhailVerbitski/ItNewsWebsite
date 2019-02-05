namespace Domain.Contracts.Models
{
    public class PostImage
    {
        public int PostId { get; set; }
        public byte[] Data { get; set; }
        public string Extension { get; set; }
    }
}
