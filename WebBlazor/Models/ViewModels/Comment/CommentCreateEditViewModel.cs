using WebBlazor.Models.ViewModels.User;

namespace WebBlazor.Models.ViewModels.Comment
{
    public class CommentCreateEditViewModel
    {
        public int CommentId { get; set; }

        public UserMiniViewModel AuthorUserMiniViewModel { get; set; }

        public string Content { get; set; } 

        public int? PostId { get; set; }
    }
}
