using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;


    }
}
