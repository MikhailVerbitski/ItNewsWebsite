using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models.ViewModels.Comment;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Implementation.Services
{
    public class ServiceOfSection
    {
        private readonly IMapper mapper;

        private readonly RepositoryOfSection repositoryOfSection;
        private readonly RepositoryOfPost repositoryOfPost;

        public ServiceOfSection(ApplicationDbContext context, IMapper mapper)
        {
            this.mapper = mapper;

            repositoryOfSection = new RepositoryOfSection(context);
            repositoryOfPost = new RepositoryOfPost(context);
        }

        public IEnumerable<SelectListItem> Get()
        {
            return repositoryOfSection.ReadMany(null).Select(a => new SelectListItem(a.Name, a.Id.ToString()));
        }
    }
}
