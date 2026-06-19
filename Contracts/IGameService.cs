using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using HangmanServer.DTOs;

namespace HangmanServer.Contracts
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IGameCallback))]
    public interface IGameService
    {
        [OperationContract(IsOneWay = true)]
        void JoinGameChannel(int matchId, int userId);

        [OperationContract(IsOneWay = true)]
        void SendGuess(int matchId, int userId, char guessLetter);

        [OperationContract(IsOneWay = true)]
        void SubmitTurnResult(int matchId, int userId, bool isCorrect, int[] correctPositions);

        [OperationContract(IsOneWay = true)]
        void LeaveMatch(int matchId, int userId);

        [OperationContract(IsOneWay = true)]
        void SendChatMessage(int matchId, string senderUsername, string message);
    }

    [ServiceContract]
    public interface IGameCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnGameStarted(GameContextDTO gameContext);

        [OperationContract(IsOneWay = true)]
        void OnGuessReceived(char guessedLetter);

        [OperationContract(IsOneWay = true)]
        void OnTurnResultReceived(TurnResultDTO turnResult);

        [OperationContract(IsOneWay = true)]
        void OnGameEnded(GameEndDTO endResult);

        [OperationContract(IsOneWay = true)]
        void OnTimerTick(int secondsLeft);

        [OperationContract(IsOneWay = true)]
        void OnChatMessageReceived(string senderUsername, string message);

        [OperationContract(IsOneWay = true)]
        void OnEvaluationError();
    }
}