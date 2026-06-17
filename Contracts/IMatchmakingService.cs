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
        Task<int> CreateMatchAsync(string username, string categoryName, string wordText, string languageCode);

        [OperationContract]
        Task<List<AvailableMatchDTO>> GetAvailableMatchesAsync(string languageCode);

        [OperationContract]
        Task<AvailableMatchDTO> GetMatchStatusAsync(int matchId);

        [OperationContract]
        Task<bool> JoinMatchAsync(int matchId, string username);

        [OperationContract]
        Task<bool> StartMatchAsync(int matchId);

        [OperationContract]
        Task<bool> LeaveMatchAsync(int matchId, bool isCreator);
    }
}