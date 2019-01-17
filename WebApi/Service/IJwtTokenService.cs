namespace WebApi.Server.Interface
{
    public interface IJwtTokenService
    {
        string BuildToken(string email);
    }
}
