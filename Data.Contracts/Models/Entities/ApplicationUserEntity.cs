using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Data.Contracts.Models.Entities
{
    public class ApplicationUserEntity : IdentityUser
    {
        public int UserProfileId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int CountOfLikes { get; set; } // CountLikes

        public UserProfileEntity UserProfile { get; set; } // UserProfile

        public string Avatar { get; set; } // Avatar
        
        public int? RoleId { get; set; }

        public RoleEntity Role { get; set; } // Access level (reader, writer, admin)

    }
}
