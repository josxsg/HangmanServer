using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using HangmanServer.BusinessLogic;
using HangmanServer.Contracts;
using HangmanServer.DataAccess;
using HangmanServer.DTOs;
using System.Threading.Tasks;

namespace HangmanServer.Services
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "AccountService" en el código, en svc y en el archivo de configuración a la vez.
    // NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione AccountService.svc o AccountService.svc.cs en el Explorador de soluciones e inicie la depuración.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class AccountService : IAccountService
    {
        public async Task<bool> RegisterUserAsync(UserDTO user, string password)
        {
            using (var context = new HangmanDBEntities())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var manager = new AccountManager(unitOfWork);
                    return await manager.RegisterUserAsync(user, password);
                }
            }
        }

        public async Task<UserDTO> LoginAsync(string username, string password)
        {
            using (var context = new HangmanDBEntities())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var manager = new AccountManager(unitOfWork);
                    return await manager.LoginAsync(username, password);
                }
            }
        }

        public async Task<UserDTO> GetUserProfileAsync(int userId)
        {
            using (var context = new HangmanDBEntities())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var manager = new AccountManager(unitOfWork);
                    return await manager.GetUserProfileAsync(userId);
                }
            }
        }

        public async Task<bool> UpdateUserProfileAsync(UserDTO userDto)
        {
            using (var context = new HangmanDBEntities())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var manager = new AccountManager(unitOfWork);
                    return await manager.UpdateUserProfileAsync(userDto);
                }
            }
        }
    }
}
