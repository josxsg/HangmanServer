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
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "CatalogService" en el código, en svc y en el archivo de configuración a la vez.
    // NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione CatalogService.svc o CatalogService.svc.cs en el Explorador de soluciones e inicie la depuración.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class CatalogService : ICatalogService
    {
        public async Task<List<CategoryDTO>> GetCategoriesAsync(string languageCode)
        {
            using (var context = new HangmanDBEntities())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var manager = new CatalogManager(unitOfWork);
                    return await manager.GetCategoriesAsync(languageCode);
                }
            }
        }

        public async Task<List<WordDTO>> GetWordsByCategoryAsync(int categoryId)
        {
            using (var context = new HangmanDBEntities())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var manager = new CatalogManager(unitOfWork);
                    return await manager.GetWordsByCategoryAsync(categoryId);
                }
            }
        }
    }
}
