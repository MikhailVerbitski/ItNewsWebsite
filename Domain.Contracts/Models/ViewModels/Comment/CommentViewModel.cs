using Domain.Contracts.Models.ViewModels.User;
using System;
using System.Collections.Generic;

namespace Domain.Contracts.Models.ViewModels.Comment
{
    public class CommentViewModel : BaseCommentViewModel
    {
        public int CountOfLikes { get; set; }

        public IEnumerable<UserMiniViewModel> Likes { get; set; }

        public bool IsUserLiked { get; set; }

        public DateTime Created { get; set; }
    }
}
