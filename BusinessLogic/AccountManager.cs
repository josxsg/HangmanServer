using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HangmanServer.DataAccess;
using HangmanServer.DTOs;

namespace HangmanServer.BusinessLogic
{
    public class AccountManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserDTO> LoginAsync(string username, string password)
        {
            var users = await _unitOfWork.Users.FindAsync(u => u.Username == username && u.PasswordHash == password);
            var userEntity = users.FirstOrDefault();

            if (userEntity == null)
            {
                return null;
            }

            return MapToDTO(userEntity);
        }

        public async Task<UserDTO> GetUserProfileAsync(int userId)
        {
            var users = await _unitOfWork.Users.FindAsync(u => u.UserID == userId);
            var userEntity = users.FirstOrDefault();

            if (userEntity == null)
            {
                return null;
            }

            return MapToDTO(userEntity);
        }

        private UserDTO MapToDTO(Users userEntity)
        {
            if (userEntity == null) return null;
            return new UserDTO
            {
                UserId = userEntity.UserID,
                Name = userEntity.Name,
                PaternalSurname = userEntity.PaternalSurname,
                MaternalSurname = userEntity.MaternalSurname,
                Username = userEntity.Username,
                Email = userEntity.Email,
                BirthDate = userEntity.BirthDate,
                PhoneNumber = userEntity.PhoneNumber
            };
        }
    }
}