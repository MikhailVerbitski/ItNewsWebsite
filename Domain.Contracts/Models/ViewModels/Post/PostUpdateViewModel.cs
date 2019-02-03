using Domain.Contracts.Models.ViewModels.Tag;
using System.Collections.Generic;

namespace Domain.Contracts.Models.ViewModels.Post
{
    public class PostUpdateViewModel : BasePostViewModel
    {
        public int? SectionId { get; set; }

        public string Section { get; set; }

        public int UserProfileId { get; set; }

        public string Content { get; set; }

        public IEnumerable<TagViewModel> Tags { get; set; }

        public IEnumerable<string> Images { get; set; }

        public bool IsFinished { get; set; }
    }
}
