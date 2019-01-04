namespace Data.Contracts.Models.Entities
{
    public class CommentEntity
    {
        public int Id { get; set; }

        public ApplicationUserEntity Author { get; set; } // Author

        public string Content { get; set; } // Content

        // Likes
    }
}
