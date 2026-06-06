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
    }
}
