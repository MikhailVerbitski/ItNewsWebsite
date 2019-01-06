using Domain.Contracts.Models.ViewModels.User;
using System.Collections.Generic;

namespace Domain.Contracts.Models.ViewModels.Post
{
    public class PostCreateEditViewModel
    {
        public int PostId { get; set; }

        public string Header { get; set; }

        public int? SectionId { get; set; }

        public string Section { get; set; }

        public UserMiniViewModel AuthorUserMiniViewModel { get; set; }

        public string Content { get; set; }

        public string BriefDesctiption { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public IEnumerable<string> Images { get; set; }
    }
}
