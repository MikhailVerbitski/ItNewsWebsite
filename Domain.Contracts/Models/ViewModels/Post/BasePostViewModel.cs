using System;

namespace Domain.Contracts.Models.ViewModels.Post
{
    public class BasePostViewModel
    {
        public int PostId { get; set; }

        public string Header { get; set; }

        public string BriefDesctiption { get; set; }

        public DateTime Created { get; set; }

        public bool BelongsToUser { get; set; }
    }
}
