﻿using WebBlazor.Models.ViewModels.Comment;
using WebBlazor.Models.ViewModels.Post;
using System.Collections.Generic;

namespace WebBlazor.Models.ViewModels.User
{
    public class UserViewModel
    {
        public string ApplicationUserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Avatar { get; set; }

        public string Role { get; set; }

        public string RoleColor { get; set; }

        public int CountOfLikes { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public virtual IEnumerable<PostMiniViewModel> Posts { get; set; }

        public virtual IEnumerable<CommentMiniViewModel> Comments { get; set; }
    }
}
