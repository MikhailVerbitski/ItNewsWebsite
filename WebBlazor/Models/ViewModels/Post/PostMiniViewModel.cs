using System;

namespace WebBlazor.Models.ViewModels.Post
{
    public class PostMiniViewModel
    {
        public int PostId { get; set; }

        public string Header { get; set; }

        public double Score { get; set; }

        public string BriefDesctiption { get; set; }

        public bool BelongsToUser { get; set; }

        public DateTime Created { get; set; }
    }
}
