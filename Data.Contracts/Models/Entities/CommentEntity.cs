using System;
using System.Collections.Generic;

namespace Data.Contracts.Models.Entities
{
    public class CommentEntity
    {
        public int Id { get; set; }

        public int? UserProfileId { get; set; }

        public string Content { get; set; }

        public int CountOfLikes { get; set; }

        public int? PostId { get; set; }

        public virtual PostEntity Post { get; set; }

        public virtual DateTime Created { get; set; }

        public virtual UserProfileEntity UserProfile { get; set; }

        public virtual IEnumerable<CommentLikeEntity> Likes { get; set; }
    }
}
