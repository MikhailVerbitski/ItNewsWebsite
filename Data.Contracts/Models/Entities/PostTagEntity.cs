namespace Data.Contracts.Models.Entities
{
    public class PostTagEntity
    {
        public int Id { get; set; }

        public int? PostId { get; set; }

        public int? TagId { get; set; }

        public virtual PostEntity Post { get; set; }

        public virtual TagEntity Tag { get; set; }
    }
}
