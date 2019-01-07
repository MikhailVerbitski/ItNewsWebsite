using System.Collections.Generic;

namespace Data.Contracts.Models.Entities
{
    public class PostEntity
    {
        public int Id { get; set; }

        public string Header { get; set; }  // Name

        public int? SectionId { get; set; }

        public virtual SectionEntity Section { get; set; } // Section (раздел)

        public long SumOfScore { get; set; } // Rating
        public int CountOfScore { get; set; } // Rating

        public bool? IsFinished { get; set; }

        public virtual IEnumerable<CommentEntity> Comments { get; set; } // Comments

        public int? UserProfileId { get; set; }

        public virtual UserProfileEntity UserProfile { get; set; } // Author

        public string Content { get; set; } // Content

        public string BriefDesctiption { get; set; } // Brief description (краткое описание)

        public virtual IEnumerable<PostTagEntity> Tags { get; set; } // Tags

        public virtual IEnumerable<ImageEntity> Images { get; set; } // Images

        public virtual IEnumerable<PostRatingEntity> PostRatings { get; set; }
    }
}
