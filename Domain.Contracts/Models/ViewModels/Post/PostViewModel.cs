using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Contracts.Models.ViewModels.Tag;
using Domain.Contracts.Models.ViewModels.User;
using System.Collections.Generic;

namespace Domain.Contracts.Models.ViewModels.Post
{
    public class PostViewModel : BasePostViewModel
    {
        public string CurrentUserId { get; set; }

        public int SectionId { get; set; }

        public string Section { get; set; }

        public double Score { get; set; }

        public byte? UserScore { get; set; } 

        public UserMiniViewModel AuthorUserMiniViewModel { get; set; }

        public string Content { get; set; }

        public IEnumerable<TagViewModel> Tags { get; set; }

        public IEnumerable<string> Images { get; set; }

        public IEnumerable<CommentViewModel> Comments { get; set; }
    }
}
