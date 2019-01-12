using System.Collections.Generic;

namespace Data.Contracts.Models.Entities
{
    public class UserProfileEntity
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUserEntity ApplicationUser { get; set; }

        public virtual IEnumerable<PostEntity> Posts { get; set; }

        public virtual IEnumerable<CommentEntity> Comments { get; set; }

        public virtual IEnumerable<PostRatingEntity> PostRatings { get; set; }
    }
}
