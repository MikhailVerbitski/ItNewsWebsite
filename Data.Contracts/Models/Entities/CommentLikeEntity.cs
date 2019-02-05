namespace Data.Contracts.Models.Entities
{
    public class CommentLikeEntity
    {
        public int Id { get; set; }

        public int? CommentId { get; set; }

        public int? UserProfileId { get; set; }

        public virtual CommentEntity Comment { get; set; }

        public virtual UserProfileEntity UserProfile { get; set; }
    }
}
