using System.Collections.Generic;

namespace Data.Contracts.Models.Entities
{
    public class PostEntity
    {
        public int Id { get; set; }

        public string Header { get; set; }  // Name

        public SectionEntity Section { get; set; } // Section (раздел)

        public long SumOfAssessment { get; set; } // Rating
        public int CountOfAssessment { get; set; } // Rating

        public IEnumerable<CommentEntity> Comments { get; set; } // Comments

        public ApplicationUserEntity ApplicationUser { get; set; } // Author

        public string Content { get; set; } // Content

        public string BriefDesctiption { get; set; } // Brief description (краткое описание)

        public IEnumerable<TagEntity> Tags { get; set; } // Tags

        public IEnumerable<string> Images { get; set; } // Images
    }
}
