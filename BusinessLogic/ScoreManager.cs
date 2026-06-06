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
            var userMatches = await _unitOfWork.Matches.FindAsync(m => (m.CreatorID == userId || m.ChallengerID == userId) && m.EndDate != null);
            var historyList = new List<MatchHistoryDTO>();

            foreach (var match in userMatches)
            {
                string result = "Desconocido";
                if (match.AbandonerID != null)
                {
                    result = "Abandonada";
                }
                else if (match.WinnerID == userId)
                {
                    result = "Ganada";
                }
                else if (match.WinnerID != null && match.WinnerID != userId)
                {
                    result = "Perdida";
                }

                historyList.Add(new MatchHistoryDTO
                {
                    MatchId = match.MatchID,
                    Date = match.EndDate ?? match.CreationDate ?? System.DateTime.Now,
                    WordText = match.Words?.WordText ?? "Desconocida",
                    RivalUsername = match.CreatorID == userId ?
                                    (match.Users1?.Username ?? "N/A") : 
                                    (match.Users?.Username ?? "N/A"),
                    Result = result
                });
            }

            return historyList;
        }
    }
}