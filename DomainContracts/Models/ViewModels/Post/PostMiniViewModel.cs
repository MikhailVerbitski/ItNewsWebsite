using Domain.Contracts.Models.ViewModels.User;

namespace Domain.Contracts.Models.ViewModels.Post
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
