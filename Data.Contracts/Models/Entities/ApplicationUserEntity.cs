using Microsoft.AspNetCore.Identity;

namespace Data.Contracts.Models.Entities
{
    public class ApplicationUserEntity : IdentityUser
    {
        public int UserProfileId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int CountOfLikes { get; set; } // CountLikes

        public UserProfile UserProfile { get; set; } // UserProfile

        // Access level (reader, writer, admin)
        // Avatar
    }
}
