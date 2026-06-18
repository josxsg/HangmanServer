using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HangmanServer.DataAccess;
using HangmanServer.DTOs;

namespace HangmanServer.BusinessLogic
{
    public class ScoreManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public ScoreManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PlayerScoreDTO> GetPlayerScoreAsync(int userId)
        {
            var userMatches = await _unitOfWork.Matches.FindAsync(m => m.CreatorID == userId || m.ChallengerID == userId);

            int totalScore = 0;
            int matchesWon = 0;
            int matchesLost = 0;
            int penalties = 0;

            foreach (var match in userMatches)
            {
                if (match.AbandonerID == userId)
                {
                    totalScore -= 3;
                    penalties++;
                    continue; 
                }

                if (match.ChallengerID == userId && match.WinnerID == userId)
                {
                    totalScore += 10;
                    matchesWon++;
                }
                else if (match.CreatorID == userId && match.WinnerID == userId)
                {
                    totalScore += 5;
                    matchesWon++;
                }
                else if (match.WinnerID != null && match.WinnerID != userId)
                {
                    matchesLost++;
                }
            }

            return new PlayerScoreDTO
            {
                TotalScore = totalScore,
                MatchesWon = matchesWon,
                MatchesLost = matchesLost,
                Penalties = penalties
            };
        }

        public async Task<List<MatchHistoryDTO>> GetMatchHistoryAsync(int userId)
        {
            var userMatches = await _unitOfWork.Matches.FindAsync(
                m => (m.CreatorID == userId || m.ChallengerID == userId) && m.EndDate != null,
                "Words"
            );

            var rivalIds = userMatches
                .Select(m => m.CreatorID == userId ? m.ChallengerID : m.CreatorID)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToList();


            var rivals = await _unitOfWork.Users.FindAsync(u => rivalIds.Contains(u.UserID));
            var rivalDictionary = rivals.ToDictionary(u => u.UserID, u => u.Username);

            var historyList = new List<MatchHistoryDTO>();

            foreach (var match in userMatches)
            {
                string result = "Desconocido";
                int points = 0;
                string rivalName = "N/A";

                if (match.AbandonerID == userId)
                {
                    result = "Abandonada";
                    points = -3;
                }
                else if (match.WinnerID == userId)
                {
                    result = "Ganada";
                    points = (match.ChallengerID == userId) ? 10 : 5;
                }
                else if (match.WinnerID != null && match.WinnerID != userId)
                {
                    result = "Perdida";
                    points = 0;
                }

                int? currentRivalId = (match.CreatorID == userId) ? match.ChallengerID : match.CreatorID;

                if (currentRivalId.HasValue && rivalDictionary.ContainsKey(currentRivalId.Value))
                {
                    rivalName = rivalDictionary[currentRivalId.Value];
                }

                historyList.Add(new MatchHistoryDTO
                {
                    MatchId = match.MatchID,
                    Date = match.EndDate ?? match.CreationDate ?? System.DateTime.Now,
                    WordText = match.Words?.WordText ?? "Desconocida",
                    RivalUsername = rivalName,
                    Result = result,
                    Points = points
                });
            }

            return historyList.OrderByDescending(h => h.Date).ToList();
        }
    }
}