using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IdentityConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //Hack for DataDirectory in ConnectionString
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AppDomain.CurrentDomain.SetData("DataDirectory", currentDirectory);

            Console.WriteLine("Seleccione la opción que desea:");
            Console.WriteLine("[1] Iniciar Sesión");
            Console.WriteLine("[2] Registrar nuevo usuario");

            var command = Console.ReadKey().KeyChar;
            Console.WriteLine();
            if (command == '1')
                CommandLogin();
            else if(command == '2')
                CommandRegister();
            else
                Console.WriteLine("Comando no reconocido. Por favor intente nuevamente.");
            
            Console.ReadLine();
        }

        private static void CommandLogin()
        {
            //Test for Identity Membership Authentication
            Console.WriteLine("Ingrese su nombre de usuario:");
            var username = Console.ReadLine();
            Console.WriteLine("Ingrese su contraseña:");
            var password = Console.ReadLine();

            using (var authRepository = new AuthRepository())
            {
                var user = authRepository.FindUser(username, password).Result;
                if (user != null) //Cambio la identidad del usuario logueado
                    System.Threading.Thread.CurrentPrincipal = GetUserAsPrincipal(user);
            }

            //Now checking for Authorization
            var currentPrincipal = System.Threading.Thread.CurrentPrincipal;
            if (currentPrincipal == null || !currentPrincipal.Identity.IsAuthenticated)
                Console.WriteLine("Usted no tiene permitido el acceso al sistema!");
            else
                Console.WriteLine("Bienvenido: {0} [{1}]. ¿Es Administrador? {2}", currentPrincipal.Identity.Name, currentPrincipal.Identity.AuthenticationType, currentPrincipal.IsInRole("admin"));

        }

        private static void CommandRegister()
        {
            //Test for Identity Membership Authentication
            Console.WriteLine("Bienvenido. Ingrese los datos para el nuevo usuario:");
            Console.WriteLine("Nombre de usuario:");
            var username = Console.ReadLine();
            Console.WriteLine("Contraseña:");
            var password = Console.ReadLine();

            using (var authRepository = new AuthRepository())
            {
                var createUserResult = authRepository.RegisterUser(username, password).Result;
                if(createUserResult.Succeeded)
                    Console.WriteLine("Usuario creado correctamente!");
                else
                {
                    Console.WriteLine("Ocurrieron los siguientes errores al intentar crear el usuario:");
                    foreach (var error in createUserResult.Errors)
                        Console.WriteLine("-{0}", error);
                }
            }
        }

        private static IPrincipal GetUserAsPrincipal(IdentityUser dbUser)
        {
            //var claims = new List<Claim>();
            //claims.Add(new Claim(ClaimTypes.Name, dbUser.UserName));
            //claims.AddRange(dbUser.Roles.Select(r => new Claim(ClaimTypes.Role, r.RoleId)));
            //var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var principal = new GenericPrincipal(new GenericIdentity(dbUser.UserName), dbUser.Roles.Select(r => r.RoleId).ToArray());
            return principal;
        }
        
    }
}
