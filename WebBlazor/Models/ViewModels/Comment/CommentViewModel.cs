using WebBlazor.Models.ViewModels.User;
using System.Collections.Generic;
using System;

namespace WebBlazor.Models.ViewModels.Comment
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }

        public UserMiniViewModel AuthorUserMiniViewModel { get; set; }

        public string Content { get; set; }

        public int CountOfLikes { get; set; }

        public IEnumerable<UserMiniViewModel> Likes { get; set; }

        public int? PostId { get; set; }

        public bool IsUserLiked { get; set; }

        public DateTime Created { get; set; }
    }
}
