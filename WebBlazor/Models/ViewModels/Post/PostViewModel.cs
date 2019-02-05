using WebBlazor.Models.ViewModels.Comment;
using WebBlazor.Models.ViewModels.User;
using System.Collections.Generic;
using System;
using WebBlazor.Models.ViewModels.Tag;

namespace WebBlazor.Models.ViewModels.Post
{
    public class PostViewModel
    {
        public string CurrentUserId { get; set; }

        public int PostId { get; set; }

        public string Header { get; set; }

        public int SectionId { get; set; }

        public string Section { get; set; }

        public double Score { get; set; }

        public byte? UserScore { get; set; }

        public UserMiniViewModel AuthorUserMiniViewModel { get; set; }

        public string Content { get; set; }

        public string BriefDesctiption { get; set; }

        public bool BelongsToUser { get; set; }

        public IEnumerable<TagViewModel> Tags { get; set; }

        public IEnumerable<string> Images { get; set; }

        public IEnumerable<CommentViewModel> Comments { get; set; }

        public DateTime Created { get; set; }
    }
}
