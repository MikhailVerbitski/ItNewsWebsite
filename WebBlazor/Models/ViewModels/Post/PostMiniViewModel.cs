using WebBlazor.Models.ViewModels.User;

namespace WebBlazor.Models.ViewModels.Post
{
    public class PostMiniViewModel
    {
        public int PostId { get; set; }

        public string Header { get; set; }

        public int Score { get; set; }
        
        public virtual UserMiniViewModel AuthorUserMiniViewModel { get; set; }

        public string BriefDesctiption { get; set; }
    }
}
