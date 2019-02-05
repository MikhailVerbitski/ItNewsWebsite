namespace Data.Contracts.Models.Entities
{
    public class ImageEntity
    {
        public int Id { get; set; }

        public string Path { get; set; }

        public int? PostId { get; set; }

        public virtual PostEntity Post { get; set; }
    }
}
