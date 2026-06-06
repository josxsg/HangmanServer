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
    public interface IMatchmakingService
    {
        [OperationContract]
        Task<List<AvailableMatchDTO>> GetAvailableMatchesAsync(string languageCode);
    }
}