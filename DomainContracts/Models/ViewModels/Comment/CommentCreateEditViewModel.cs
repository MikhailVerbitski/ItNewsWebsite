using Domain.Contracts.Models.ViewModels.User;

namespace Domain.Contracts.Models.ViewModels.Comment
{
    public class CommentCreateEditViewModel
    {
        public int CommentId { get; set; }

        public virtual UserMiniViewModel AuthorUserMiniViewModel { get; set; }

        public string Content { get; set; } 

        public int? PostId { get; set; }
    }
}
