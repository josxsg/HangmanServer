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
    public interface IScoreService
    {
        [OperationContract]
        Task<PlayerScoreDTO> GetPlayerScoreAsync(int userId);

        [OperationContract]
        Task<List<MatchHistoryDTO>> GetMatchHistoryAsync(int userId);
    }
}