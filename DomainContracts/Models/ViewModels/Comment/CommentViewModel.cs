using Domain.Contracts.Models.ViewModels.User;
using System.Collections.Generic;

namespace Domain.Contracts.Models.ViewModels.Comment
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }

        public virtual UserMiniViewModel AuthorUserMiniViewModel { get; set; }

        public string Content { get; set; }

        public int CountOfLikes { get; set; }

        public virtual IEnumerable<UserMiniViewModel> Likes { get; set; }

        public int? PostId { get; set; }
    }
}
