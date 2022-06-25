using System.Security.Claims;

namespace MAction.AspNetIdentity.Base
{
    public interface IJWTService
    {
        Task<string> GenerateTokenAsync(object userId);
    }
}