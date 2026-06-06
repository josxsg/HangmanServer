using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;
using HangmanServer.DTOs;

namespace HangmanServer.Contracts
{
    [ServiceContract]
    public interface ICatalogService
    {
        [OperationContract]
        Task<List<CategoryDTO>> GetCategoriesAsync(string languageCode);

        [OperationContract]
        Task<List<WordDTO>> GetWordsByCategoryAsync(int categoryId);
    }
}