using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Domain.Contracts.Models.ViewModels.User
{
    public class UserEditViewModel
    {
        public string ApplicationUserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public int? UserProfileId { get; set; }

        public IFormFile Avatar { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}
