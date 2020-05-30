using AutoMapper;

namespace Domain.Implementation.Services
{
    class ServiceOfMessage
    {
        private readonly IMapper mapper;
        private readonly ServiceOfImage serviceOfImage;
        private readonly ServiceOfUser serviceOfUser;

        public ServiceOfMessage(
            IMapper mapper,
            ServiceOfImage serviceOfImage,
            ServiceOfUser serviceOfUser
            )
        {
            this.mapper = mapper;
            this.serviceOfImage = serviceOfImage;
            this.serviceOfUser = serviceOfUser;
        }
    }
}
