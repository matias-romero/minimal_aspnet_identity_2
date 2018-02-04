using Microsoft.AspNet.Identity.EntityFramework;

namespace TestIdentity
{
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext()
            : base("AuthContext")
        {

        }
    }

}