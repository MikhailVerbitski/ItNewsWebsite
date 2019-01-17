using WebBlazor.Models.ViewModels.User;

namespace WebBlazor.Models.ViewModels.Comment
{
    public class CommentMiniViewModel
    {
        public int CommentId { get; set; }

        public virtual UserMiniViewModel AuthorUserMiniViewModel { get; set; }

        public string Content { get; set; }

        public int CountOfLikes { get; set; }

        public int? PostId { get; set; }
    }
}
