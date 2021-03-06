﻿using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Contracts.Models.ViewModels.Post;
using System;
using System.Collections.Generic;

namespace Domain.Contracts.Models.ViewModels.User
{
    public class UserViewModel
    {
        public string ApplicationUserId { get; set; }

        public int UserProfileId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Avatar { get; set; }

        public int CountOfLikes { get; set; }

        public string Email { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public bool IsCurrentUser { get; set; }

        public DateTime Created { get; set; }

        public UserRole Role { get; set; }

        public List<UserClaim> UserClaims { get; set; }

        public List<UserClaim> AllClaims { get; set; }

        public List<UserRole> AllRoles { get; set; }
    }
}
