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

            var rivalDictionary = await BuildRivalDictionaryAsync(userMatches, userId);
            var historyList = new List<MatchHistoryDTO>();

            foreach (var match in userMatches)
            {
                var (result, points) = EvaluateMatchResult(match, userId);
                string rivalName = GetRivalName(match, userId, rivalDictionary);

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

        private async Task<Dictionary<int, string>> BuildRivalDictionaryAsync(IEnumerable<Matches> userMatches, int userId)
        {
            var rivalIds = userMatches
                .Select(m => m.CreatorID == userId ? m.ChallengerID : m.CreatorID)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToList();

            var rivals = await _unitOfWork.Users.FindAsync(u => rivalIds.Contains(u.UserID));
            return rivals.ToDictionary(u => u.UserID, u => u.Username);
        }

        private static (string Result, int Points) EvaluateMatchResult(Matches match, int userId)
        {
            if (match.AbandonerID == userId)
            {
                return ("Abandonada", -3);
            }

            if (match.WinnerID == userId)
            {
                int points = (match.ChallengerID == userId) ? 10 : 5;
                return ("Ganada", points);
            }

            if (match.WinnerID != null && match.WinnerID != userId)
            {
                return ("Perdida", 0);
            }

            return ("Desconocido", 0);
        }

        private static string GetRivalName(Matches match, int userId, Dictionary<int, string> rivalDictionary)
        {
            int? currentRivalId = (match.CreatorID == userId) ? match.ChallengerID : match.CreatorID;

            if (currentRivalId.HasValue && rivalDictionary.TryGetValue(currentRivalId.Value, out string name))
            {
                return name;
            }

            return "N/A";
        }
    }
}