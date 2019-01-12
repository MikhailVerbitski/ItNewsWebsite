using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Domain.Contracts.Models.ViewModels.Account
{
    public class RegisterViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Login { get; set; }

        public string Email { get; set; }
        
        public string Password { get; set; }

        //public string ConfirmPassword { get; set; }

        public IFormFile Avatar { get; set; }

        public string ReturnUrl { get; set; }
    }
}
