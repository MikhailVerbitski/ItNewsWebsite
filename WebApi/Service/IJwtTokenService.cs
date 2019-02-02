using System.Collections.Generic;
using System.Security.Claims;

namespace WebApi.Server.Interface
{
    public interface IJwtTokenService
    {
        string BuildToken(string email, List<Claim> claims);
    }
}
