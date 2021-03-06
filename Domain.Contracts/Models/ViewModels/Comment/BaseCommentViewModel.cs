﻿using Domain.Contracts.Models.ViewModels.User;

namespace Domain.Contracts.Models.ViewModels.Comment
{
    public class BaseCommentViewModel
    {
        public int CommentId { get; set; }

        public UserMiniViewModel AuthorUserMiniViewModel { get; set; }

        public string Content { get; set; }

        public int? PostId { get; set; }

        public bool BelongsToUser { get; set; }
    }
}
