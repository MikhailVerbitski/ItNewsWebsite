using System.Collections.Generic;

namespace Data.Contracts.Models.Entities
{
    public class CommentEntity
    {
        public int Id { get; set; }

        public int? UserProfileId { get; set; }

        public virtual UserProfileEntity UserProfile { get; set; } // Author

        public string Content { get; set; } // Content

        public int CountOfLikes { get; set; }

        public virtual IEnumerable<CommentLikeEntity> Likes { get; set; } // Likes

        public int? PostId { get; set; }

        public PostEntity Post { get; set; }
    }
}
