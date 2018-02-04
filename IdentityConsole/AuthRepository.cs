using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IdentityConsole
{
    public interface IAuthRepository : IDisposable
    {
        Task<IdentityResult> RegisterUser(string userName, string password);
        Task<IdentityUser> FindUser(string userName, string password);
    }

    public class AuthRepository : IAuthRepository
    {
        private readonly AuthContext _ctx;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthRepository()
        {
            _ctx = new AuthContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
        }

        public async Task<IdentityResult> RegisterUser(string userName, string password)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = userName
            };

            var result = await _userManager.CreateAsync(user, password);

            return result;
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);

            return user;
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }
    }
}