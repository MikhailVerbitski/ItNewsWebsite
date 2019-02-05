﻿using System;
using WebBlazor.Models.ViewModels.User;

namespace WebBlazor.Models.ViewModels.Post
{
    public class PostCompactViewModel
    {
        public int PostId { get; set; }

        public string Header { get; set; }

        public double Score { get; set; }

        public UserMiniViewModel AuthorUserMiniViewModel { get; set; }

        public string BriefDesctiption { get; set; }

        public bool BelongsToUser { get; set; }

        public DateTime Created { get; set; }
    }
}
