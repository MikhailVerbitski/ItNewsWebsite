using Microsoft.AspNetCore.Identity;

namespace Data.Contracts.Models.Entities
{
    public class RoleEntity : IdentityRole
    {
        public RoleEntity()
        { }
        public RoleEntity(string name) : base(name)
        { }

        public int Priority { get; set; }

        public string Color { get; set; }
    }
}
