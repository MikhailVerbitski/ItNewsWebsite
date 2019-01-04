using System.Collections.Generic;

namespace Data.Contracts.Models.Entities
{
    public class UserProfileEntity
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUserEntity ApplicationUser { get; set; }

        public IEnumerable<PostEntity> Posts { get; set; } //Posts

        public IEnumerable<CommentEntity> Comments { get; set; } //Comments
    }
}
