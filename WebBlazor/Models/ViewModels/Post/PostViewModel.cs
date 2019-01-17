using WebBlazor.Models.ViewModels.Comment;
using WebBlazor.Models.ViewModels.User;
using System.Collections.Generic;

namespace WebBlazor.Models.ViewModels.Post
{
    public class PostViewModel
    {
        public int PostId { get; set; }

        public string Header { get; set; }

        public int SectionId { get; set; }

        public string Section { get; set; }

        public double Score { get; set; }

        public byte? UserScore { get; set; } 

        public virtual UserMiniViewModel AuthorUserMiniViewModel { get; set; }

        public string Content { get; set; }

        public string BriefDesctiption { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public IEnumerable<string> Images { get; set; }

        public IEnumerable<CommentViewModel> Comments { get; set; }
    }
}
