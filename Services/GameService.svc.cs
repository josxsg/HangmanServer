using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using HangmanServer.BusinessLogic;
using HangmanServer.Contracts;

namespace HangmanServer.Services
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "GameService" en el código, en svc y en el archivo de configuración a la vez.
    // NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione GameService.svc o GameService.svc.cs en el Explorador de soluciones e inicie la depuración.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class GameService : IGameService
    {
        public void JoinGameChannel(int matchId, int userId)
        {
            try
            {
                IGameCallback callbackChannel = OperationContext.Current.GetCallbackChannel<IGameCallback>();

                GameManager.JoinRoom(matchId, userId, callbackChannel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error JoinGameChannel: {ex.Message}");
                throw new FaultException(ex.Message);
            }
        }

        public void SendGuess(int matchId, int userId, char guessLetter)
        {
            try
            {
                GameManager.ProcessGuess(matchId, userId, guessLetter);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error SendGuess: {ex.Message}");
            }
        }

        public void SubmitTurnResult(int matchId, int userId, bool isCorrect, int[] correctPositions)
        {
            try
            {
                GameManager.ProcessTurnResult(matchId, userId, isCorrect, correctPositions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error SubmitTurnResult: {ex.Message}");
            }
        }

        public void LeaveMatch(int matchId, int userId)
        {
            try
            {
                GameManager.LeaveMatch(matchId, userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error LeaveMatch: {ex.Message}");
            }
        }
    }
}
