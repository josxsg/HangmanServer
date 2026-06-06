using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HangmanServer.DataAccess;
using HangmanServer.DTOs;

namespace HangmanServer.BusinessLogic
{
    public class MatchmakingManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public MatchmakingManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AvailableMatchDTO>> GetAvailableMatchesAsync(string languageCode)
        {
            var matches = await _unitOfWork.Matches.FindAsync(m => m.ChallengerID == null && m.StatusID == 1);

            var availableMatchesList = new List<AvailableMatchDTO>();

            foreach (var match in matches)
            {
                availableMatchesList.Add(new AvailableMatchDTO
                {
                    MatchId = match.MatchID,
                    CreatorUsername = match.Users?.Username ?? "Desconocido",
                    CategoryName = match.Words?.Categories?.CategoryName ?? "General",
                    CreationDate = match.CreationDate ?? System.DateTime.Now
                });
            }

            return availableMatchesList;
        }
    }
}