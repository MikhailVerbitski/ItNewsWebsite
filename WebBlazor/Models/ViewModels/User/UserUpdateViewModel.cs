﻿namespace WebBlazor.Models.ViewModels.User
{
    public class UserUpdateViewModel
    {
        public string ApplicationUserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public UserRole Role { get; set; }
    }
}
