using WebApplication2.Entities;

namespace WebApplication2.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser user);
    }
}
