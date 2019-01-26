using Domain.Contracts.Models.ViewModels.User;

namespace Domain.Contracts.Models.ViewModels.Post
{
    public class PostCompactViewModel : BasePostViewModel
    {
        public int Score { get; set; }

        public virtual UserMiniViewModel AuthorUserMiniViewModel { get; set; }
    }
}
