using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Contracts.Models.ViewModels.User
{
    public class UserEditViewModel
    {
        public string ApplicationUserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public int? UserProfileId { get; set; }

        public string Avatar { get; set; }

        public int? RoleId { get; set; }

        public string Role { get; set; }
    }
}
