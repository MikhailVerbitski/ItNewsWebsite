using System;

namespace Domain.Contracts.Models.ViewModels.Comment
{
    public class CommentMiniViewModel : BaseCommentViewModel
    {
        public int CountOfLikes { get; set; }

        public string PostHeader { get; set; }

        public DateTime Created { get; set; }
    }
}
