using System;

namespace Domain.Contracts.Models.ViewModels.User
{
    public class UserMiniViewModel
    {
        public string ApplicationUserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Avatar { get; set; }

        public int CountOfLikes { get; set; }

        public string Login { get; set; }

        public UserRole Role { get; set; }

        public DateTime Created { get; set; }
    }
}
