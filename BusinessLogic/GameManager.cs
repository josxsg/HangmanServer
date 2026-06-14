using System;
using System.Collections.Concurrent;
using HangmanServer.Contracts;
using HangmanServer.DTOs;
using HangmanServer.DataAccess;

namespace HangmanServer.BusinessLogic
{
    public static class GameManager
    {
        private static readonly ConcurrentDictionary<int, GameRoom> _activeRooms = new ConcurrentDictionary<int, GameRoom>();

        public static void CreateRoom(GameContextDTO context)
        {
            if (!_activeRooms.ContainsKey(context.MatchId))
            {
                var newRoom = new GameRoom(context);

                newRoom.RoomFinished += OnRoomFinished;

                _activeRooms.TryAdd(context.MatchId, newRoom);
            }
        }

        public static void JoinRoom(int matchId, int userId, IGameCallback callback)
        {
            if (_activeRooms.TryGetValue(matchId, out GameRoom room))
            {
                room.Join(userId, callback);
            }
            else
            {
                throw new InvalidOperationException("La sala solicitada no existe o ya ha sido cerrada.");
            }
        }

        public static void ProcessGuess(int matchId, int userId, char guess)
        {
            if (_activeRooms.TryGetValue(matchId, out GameRoom room))
            {
                room.ProcessGuess(userId, guess);
            }
        }

        public static void ProcessTurnResult(int matchId, int userId, bool isCorrect, int[] positions)
        {
            if (_activeRooms.TryGetValue(matchId, out GameRoom room))
            {
                room.ProcessTurnResult(userId, isCorrect, positions);
            }
        }

        public static void LeaveMatch(int matchId, int userId)
        {
            if (_activeRooms.TryGetValue(matchId, out GameRoom room))
            {
                room.LeaveMatch(userId);
            }
        }

        private static async void OnRoomFinished(int matchId, GameEndDTO result)
        {
            if (_activeRooms.TryRemove(matchId, out GameRoom room))
            {
                room.RoomFinished -= OnRoomFinished; 
            }

            try
            {
                using (var context = new HangmanDBEntities())
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var match = await unitOfWork.Matches.GetByIdAsync(matchId);

                    if (match != null)
                    {
                        match.StatusID = (result.Reason == MatchEndReason.Abandoned) ? 4 : 3;
                        match.EndDate = DateTime.Now;
                        if (result.WinnerId > 0)
                        {
                            match.WinnerID = result.WinnerId;
                        }
                        if (result.Reason == MatchEndReason.Abandoned && result.PenalizedUserId != 0)
                        {
                            match.AbandonerID = result.PenalizedUserId;
                        }

                        await unitOfWork.CompleteAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar el resultado de la partida {matchId}: {ex.Message}");
            }
        }
    }
}