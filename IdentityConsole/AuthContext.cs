using Microsoft.AspNet.Identity.EntityFramework;

namespace IdentityConsole
{
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext()
            : base("AuthContext")
        {

        }
    }

}