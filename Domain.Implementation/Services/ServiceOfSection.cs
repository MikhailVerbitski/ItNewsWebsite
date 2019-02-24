using AutoMapper;
using Data.Implementation;
using Data.Implementation.Repositories;
using Search.Implementation;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Implementation.Services
{
    public class ServiceOfSection
    {
        private readonly IMapper mapper;

        private readonly RepositoryOfSection repositoryOfSection;
        private readonly RepositoryOfPost repositoryOfPost;

        public ServiceOfSection(ApplicationDbContext context, IMapper mapper, ServiceOfSearch serviceOfSearch)
        {
            this.mapper = mapper;

            repositoryOfSection = new RepositoryOfSection(context);
            repositoryOfPost = new RepositoryOfPost(context, serviceOfSearch);
        }

        public List<string> Get()
        {
            return repositoryOfSection.ReadMany(null).Select(a => a.Name).ToList();
        }
    }
}
