using Microsoft.AspNetCore.Identity;

namespace Data.Contracts.Models.Entities
{
    public class ApplicationUserEntity : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int CountOfLikes { get; set; } // CountLikes
        
        public int? UserProfileId { get; set; }

        public virtual UserProfileEntity UserProfile { get; set; } // UserProfile

        public string Avatar { get; set; } // Avatar
        
        public int? RoleId { get; set; }

        public virtual RoleEntity Role { get; set; } // Access level (reader, writer, admin)

    }
}
