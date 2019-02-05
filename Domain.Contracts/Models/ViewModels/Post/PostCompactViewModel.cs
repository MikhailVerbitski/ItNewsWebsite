using Domain.Contracts.Models.ViewModels.User;

namespace Domain.Contracts.Models.ViewModels.Post
{
    public class PostCompactViewModel : BasePostViewModel
    {
        public double Score { get; set; }

        public virtual UserMiniViewModel AuthorUserMiniViewModel { get; set; }
    }
}
