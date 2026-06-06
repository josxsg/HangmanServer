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
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "ScoreService" en el código, en svc y en el archivo de configuración a la vez.
    // NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione ScoreService.svc o ScoreService.svc.cs en el Explorador de soluciones e inicie la depuración.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ScoreService : IScoreService
    {
        public async Task<PlayerScoreDTO> GetPlayerScoreAsync(int userId)
        {
            using (var context = new HangmanDBEntities())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var manager = new ScoreManager(unitOfWork);
                    return await manager.GetPlayerScoreAsync(userId);
                }
            }
        }

        public async Task<List<MatchHistoryDTO>> GetMatchHistoryAsync(int userId)
        {
            using (var context = new HangmanDBEntities())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var manager = new ScoreManager(unitOfWork);
                    return await manager.GetMatchHistoryAsync(userId);
                }
            }
        }
    }
}
