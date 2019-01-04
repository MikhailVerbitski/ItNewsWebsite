using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Contracts.Models.Entities
{
    public class RoleEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ApplicationUserEntity> Users { get; set; }
        public RoleEntity()
        {
            Users = new List<ApplicationUserEntity>();
        }
    }
}
