using System.Collections.Generic;

namespace Data.Contracts.Models.Entities
{
    public class RoleEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual IEnumerable<ApplicationUserEntity> ApplicationUsers { get; set; }
        public RoleEntity()
        {
            ApplicationUsers = new List<ApplicationUserEntity>();
        }
    }
}
