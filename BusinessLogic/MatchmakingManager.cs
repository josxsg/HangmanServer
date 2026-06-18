using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<int> CreateMatchAsync(string username, string categoryName, string wordText, string languageCode)
        {
            var creatorUser = await FetchCreatorUserAsync(username);
            var selectedWord = await FetchSelectedWordAsync(wordText);

            var newMatch = new Matches
            {
                CreatorID = creatorUser.UserID,
                WordID = selectedWord.WordID,
                CreationDate = System.DateTime.Now,
                StatusID = 1,
                ChallengerID = null
            };

            _unitOfWork.Matches.Add(newMatch);
            await _unitOfWork.CompleteAsync();

            return newMatch.MatchID;
        }

        private async Task<Users> FetchCreatorUserAsync(string username)
        {
            var user = (await _unitOfWork.Users.FindAsync(u => u.Username == username)).FirstOrDefault();
            if (user == null)
            {
                throw new Exception("Usuario creador no encontrado.");
            }
            return user;
        }

        private async Task<Words> FetchSelectedWordAsync(string wordText)
        {
            var word = (await _unitOfWork.Words.FindAsync(w => w.WordText == wordText)).FirstOrDefault();
            if (word == null)
            {
                throw new Exception("La palabra seleccionada no existe en el catálogo.");
            }
            return word;
        }

        private async Task<int> GenerateUniqueMatchIdAsync()
        {
            var random = new Random();
            int newMatchId;
            bool idExists;

            do
            {
                newMatchId = random.Next(1000, 10000);
                var existing = await _unitOfWork.Matches.FindAsync(m => m.MatchID == newMatchId);
                idExists = existing.Any();
            } while (idExists);

            return newMatchId;
        }

        public async Task<List<AvailableMatchDTO>> GetAvailableMatchesAsync(string languageCode)
        {
            var matches = await _unitOfWork.Matches.FindAsync(m => m.ChallengerID == null && m.StatusID == 1);

            var availableMatchesList = new List<AvailableMatchDTO>();

            foreach (var match in matches)
            {
                var creator = (await _unitOfWork.Users.FindAsync(u => u.UserID == match.CreatorID)).FirstOrDefault();
                string creatorUsername = creator != null ? creator.Username : "Desconocido";

                var word = (await _unitOfWork.Words.FindAsync(w => w.WordID == match.WordID)).FirstOrDefault();
                string categoryName = "General";

                if (word != null)
                {
                    var category = (await _unitOfWork.Categories.FindAsync(c => c.CategoryID == word.CategoryID)).FirstOrDefault();
                    if (category != null)
                    {
                        categoryName = category.CategoryName;
                    }
                }

                availableMatchesList.Add(new AvailableMatchDTO
                {
                    MatchId = match.MatchID,
                    CreatorUsername = creatorUsername,
                    CategoryName = categoryName,
                    CreationDate = match.CreationDate ?? System.DateTime.Now
                });
            }

            return availableMatchesList;
        }

        public async Task<AvailableMatchDTO> GetMatchStatusAsync(int matchId)
        {
            var match = (await _unitOfWork.Matches.FindAsync(m => m.MatchID == matchId)).FirstOrDefault();

            if (match == null)
            {
                return null;
            }

            string challengerName = null;
            if (match.ChallengerID != null)
            {
                var challenger = (await _unitOfWork.Users.FindAsync(u => u.UserID == match.ChallengerID)).FirstOrDefault();
                challengerName = challenger?.Username;
            }

            return new AvailableMatchDTO
            {
                MatchId = match.MatchID,
                CreatorUsername = match.CreatorUser?.Username ?? "Desconocido",
                CategoryName = match.Words?.Categories?.CategoryName ?? "General",
                CreationDate = match.CreationDate ?? System.DateTime.Now,
                ChallengerUsername = challengerName,
                StatusId = match.StatusID
            };
        }

        public async Task<bool> JoinMatchAsync(int matchId, string username)
        {
            var match = (await _unitOfWork.Matches.FindAsync(m => m.MatchID == matchId)).FirstOrDefault();

            if (match == null || match.StatusID != 1 || match.ChallengerID != null)
            {
                return false;
            }

            var challengerUser = (await _unitOfWork.Users.FindAsync(u => u.Username == username)).FirstOrDefault();
            if (challengerUser == null)
            {
                return false;
            }

            match.ChallengerID = challengerUser.UserID;

            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> StartMatchAsync(int matchId)
        {
            var match = (await _unitOfWork.Matches.FindAsync(m => m.MatchID == matchId)).FirstOrDefault();
            if (match == null || match.StatusID != 1 || match.ChallengerID == null)
            {
                return false;
            }
            var word = (await _unitOfWork.Words.FindAsync(w => w.WordID == match.WordID)).FirstOrDefault();
            if (word == null)
            {
                return false;
            }
            var category = (await _unitOfWork.Categories.FindAsync(c => c.CategoryID == word.CategoryID)).FirstOrDefault();
            string categoryName = category != null ? category.CategoryName : "General";
            match.StatusID = 2;
            await _unitOfWork.CompleteAsync();
            var gameContext = new GameContextDTO
            {
                MatchId = match.MatchID,
                CreatorId = match.CreatorID,
                ChallengerId = match.ChallengerID.Value,
                WordLength = word.Length,              
                CategoryName = categoryName,           
                WordDescription = word.Description,
                SecretWord = word.WordText
            };
            GameManager.CreateRoom(gameContext);
            return true;
        }

        public async Task<bool> LeaveMatchAsync(int matchId, bool isCreator)
        {
            var match = (await _unitOfWork.Matches.FindAsync(m => m.MatchID == matchId)).FirstOrDefault();

            if (match == null || match.StatusID != 1)
            {
                return false;
            }

            if (isCreator)
            {
                match.StatusID = 4;
            }
            else
            {
                match.ChallengerID = null;
                match.StatusID = 1;
            }

            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<int> GetUserIdByUsernameAsync(string username)
        {
            var user = (await _unitOfWork.Users.FindAsync(u => u.Username == username)).FirstOrDefault();
            return user?.UserID ?? 0;
        }
    }
}