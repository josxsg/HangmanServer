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
    public interface IAccountService
    {
        [OperationContract]
        Task<UserDTO> LoginAsync(string username, string password);

        [OperationContract]
        Task<UserDTO> GetUserProfileAsync(int userId);
    }
}