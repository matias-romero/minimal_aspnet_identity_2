using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TestIdentity
{
    public class BasicAuthHandler : DelegatingHandler
    {
        private IdentityUser _authenticatedUser = null;

        //Method to validate credentials from Authorization header value
        private async Task<bool> ValidateCredentials(AuthenticationHeaderValue authenticationHeaderVal)
        {
            try
            {
                if (authenticationHeaderVal != null && !string.IsNullOrEmpty(authenticationHeaderVal.Parameter))
                {
                    var fromBase64String = Convert.FromBase64String(authenticationHeaderVal.Parameter);
                    string[] decodedCredentials = Encoding.ASCII.GetString(fromBase64String).Split(':');

                    //now decodedCredentials[0] will contain
                    //username and decodedCredentials[1] will
                    //contain password.
                    var username = decodedCredentials[0];
                    var password = decodedCredentials[1];
                    using (var repository = this.GetUserRepository())
                    {
                        var user = await repository.FindUser(username, password);

                        _authenticatedUser = user;
                        return user != null;
                    }


                }
                return false;//request not authenticated.
            }
            catch
            {
                return false;
            }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //if the credentials are validated,
            //set CurrentPrincipal and Current.User
            if (await ValidateCredentials(request.Headers.Authorization))
            {
                //var claims = new List<Claim>();
                //claims.Add(new Claim(ClaimTypes.Name, _authenticatedUser.UserName));
                //claims.AddRange(_authenticatedUser.Roles.Select(r => new Claim(ClaimTypes.Role, r.RoleId)));
                //var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
                var principal = new GenericPrincipal(new GenericIdentity(_authenticatedUser.UserName), _authenticatedUser.Roles.Select(r => r.RoleId).ToArray());
                Thread.CurrentPrincipal = principal;
                HttpContext.Current.User = principal;
            }
            //Execute base.SendAsync to execute default
            //actions and once it is completed,
            //capture the response object and add
            //WWW-Authenticate header if the request
            //was marked as unauthorized.

            //Allow the request to process further down the pipeline
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized
                && !response.Headers.Contains("WWW-Authenticate"))
            {
                response.Headers.Add("WWW-Authenticate", "Basic");
            }

            return response;
        }

        private IAuthRepository GetUserRepository()
        {
            return new AuthRepository();
        }

    }
}