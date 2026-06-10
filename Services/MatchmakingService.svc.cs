using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using HangmanServer.BusinessLogic;
using HangmanServer.DataAccess;
using HangmanServer.DTOs;
using System.Threading.Tasks;
using HangmanServer.Contracts;

namespace HangmanServer.Services
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "MatchmakingService" en el código, en svc y en el archivo de configuración a la vez.
    // NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione MatchmakingService.svc o MatchmakingService.svc.cs en el Explorador de soluciones e inicie la depuración.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class MatchmakingService : IMatchmakingService
    {
        public async Task<int> CreateMatchAsync(string username, string categoryName, string wordText, string languageCode)
        {
            using (var context = new HangmanDBEntities())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var manager = new MatchmakingManager(unitOfWork);
                    return await manager.CreateMatchAsync(username, categoryName, wordText, languageCode);
                }
            }
        }

        public async Task<List<AvailableMatchDTO>> GetAvailableMatchesAsync(string languageCode)
        {
            using (var context = new HangmanDBEntities())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var manager = new MatchmakingManager(unitOfWork);
                    return await manager.GetAvailableMatchesAsync(languageCode);
                }
            }
        }

        public async Task<AvailableMatchDTO> GetMatchStatusAsync(int matchId)
        {
            using (var context = new HangmanDBEntities())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var manager = new MatchmakingManager(unitOfWork);
                    return await manager.GetMatchStatusAsync(matchId);
                }
            }
        }

        public async Task<bool> JoinMatchAsync(int matchId, string username)
        {
            using (var context = new HangmanDBEntities())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var manager = new MatchmakingManager(unitOfWork);
                    return await manager.JoinMatchAsync(matchId, username);
                }
            }
        }

        public async Task<bool> StartMatchAsync(int matchId)
        {
            using (var context = new HangmanDBEntities())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var manager = new MatchmakingManager(unitOfWork);
                    return await manager.StartMatchAsync(matchId);
                }
            }
        }

        public async Task<bool> LeaveMatchAsync(int matchId, bool isCreator) 
        {
            using (var context = new HangmanDBEntities())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var manager = new MatchmakingManager(unitOfWork);
                    return await manager.LeaveMatchAsync(matchId, isCreator);
                }
            }
        }
    }
}
