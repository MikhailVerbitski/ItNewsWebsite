using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Implementation.Services
{
    public class ServiceOfRole
    {
        private readonly RepositoryOfRole repositoryOfRole;

        public ServiceOfRole(ApplicationDbContext context)
        {
            repositoryOfRole = new RepositoryOfRole(context);
        }

        public IEnumerable<RoleEntity> Get()
        {
            var roles = repositoryOfRole.ReadMany(null).ToList();
            return roles;
        }
        public IEnumerable<SelectListItem> GetSelectListItem()
        {
            var roles = Get().Select(a => new SelectListItem(a.Name, a.Id.ToString())).ToList();
            return roles;
        }
        
    }
}
