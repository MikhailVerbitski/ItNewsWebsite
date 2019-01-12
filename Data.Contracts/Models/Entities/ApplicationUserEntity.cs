using Microsoft.AspNetCore.Identity;

namespace Data.Contracts.Models.Entities
{
    public class ApplicationUserEntity : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int CountOfLikes { get; set; }
        
        public int? UserProfileId { get; set; }

        public virtual UserProfileEntity UserProfile { get; set; }

        public string Avatar { get; set; }
    }
}
