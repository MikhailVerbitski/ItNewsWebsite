using System;
using System.Collections.Generic;

namespace Data.Contracts.Models.Entities
{
    public class PostEntity
    {
        public int Id { get; set; }

        public string Header { get; set; }

        public int? SectionId { get; set; }

        public virtual SectionEntity Section { get; set; }

        public long SumOfScore { get; set; }
        public int CountOfScore { get; set; }

        public bool IsFinished { get; set; }

        public virtual IEnumerable<CommentEntity> Comments { get; set; }

        public int? UserProfileId { get; set; }

        public virtual UserProfileEntity UserProfile { get; set; }

        public string Content { get; set; }

        public string BriefDesctiption { get; set; }

        public DateTime Created { get; set; }

        public virtual IEnumerable<PostTagEntity> Tags { get; set; }

        public virtual IEnumerable<ImageEntity> Images { get; set; }

        public virtual IEnumerable<PostRatingEntity> PostRatings { get; set; }
    }
}
