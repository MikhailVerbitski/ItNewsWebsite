using System.Collections.Generic;
using WebBlazor.Models.ViewModels.Tag;

namespace WebBlazor.Models.ViewModels.Post
{
    public class PostCreateEditViewModel
    {
        public int PostId { get; set; }

        public string Header { get; set; }

        public int? SectionId { get; set; }

        public string Section { get; set; }
        
        public int UserProfileId { get; set; }

        public string Content { get; set; }

        public string BriefDesctiption { get; set; }

        public IEnumerable<TagViewModel> Tags { get; set; }

        public IEnumerable<string> Images { get; set; }

        public bool IsFinished { get; set; }
    }
}
