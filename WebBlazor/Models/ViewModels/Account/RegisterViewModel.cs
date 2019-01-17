using Microsoft.AspNetCore.Http;

namespace WebBlazor.Models.ViewModels.Account
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
