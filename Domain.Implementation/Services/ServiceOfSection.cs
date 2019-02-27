using AutoMapper;
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

        public ServiceOfSection(IMapper mapper, ServiceOfSearch serviceOfSearch, RepositoryOfSection repositoryOfSection, RepositoryOfPost repositoryOfPost)
        {
            this.mapper = mapper;
            this.repositoryOfSection = repositoryOfSection;
            this.repositoryOfPost = repositoryOfPost;
        }

        public List<string> Get()
        {
            return repositoryOfSection.ReadMany(null).Select(a => a.Name).ToList();
        }
    }
}
