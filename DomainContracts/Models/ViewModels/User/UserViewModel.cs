using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Contracts.Models.ViewModels.Post;
using System.Collections.Generic;

namespace Domain.Contracts.Models.ViewModels.User
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

        public virtual IEnumerable<PostCompactViewModel> Posts { get; set; }

        public virtual IEnumerable<CommentMiniViewModel> Comments { get; set; }
    }
}
