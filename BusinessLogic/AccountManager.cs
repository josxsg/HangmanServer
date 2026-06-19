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

        public async Task<bool> RegisterUserAsync(UserDTO userDto, string password)
        {
            var existing = await _unitOfWork.Users.FindAsync(u => u.Email == userDto.Email || u.Username == userDto.Username);
            if (existing.Any()) return false;

            _unitOfWork.Users.Add(MapToEntity(userDto, password));
            await _unitOfWork.CompleteAsync();

            return true;
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
        public async Task<bool> UpdateUserProfileAsync(UserDTO userDto, string newPassword)
        {
            var users = await _unitOfWork.Users.FindAsync(u => u.UserID == userDto.UserId);
            var userEntity = users.FirstOrDefault();

            if (userEntity == null) return false;

            var existingUsername = await _unitOfWork.Users.FindAsync(u => u.Username == userDto.Username && u.UserID != userDto.UserId);
            if (existingUsername.Any()) return false;

            userEntity.Name = userDto.Name;
            userEntity.PaternalSurname = userDto.PaternalSurname;
            userEntity.MaternalSurname = userDto.MaternalSurname ?? string.Empty;
            userEntity.BirthDate = userDto.BirthDate;
            userEntity.Username = userDto.Username;
            userEntity.PhoneNumber = userDto.PhoneNumber;

            if (!string.IsNullOrEmpty(newPassword))
            {
                userEntity.PasswordHash = newPassword;
            }

            await _unitOfWork.CompleteAsync();

            return true;
        }

        private Users MapToEntity(UserDTO dto, string password) => new Users
        {
            Name = dto.Name,
            PaternalSurname = dto.PaternalSurname,
            MaternalSurname = dto.MaternalSurname ?? string.Empty,
            BirthDate = dto.BirthDate,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            Username = dto.Username,
            PasswordHash = password
        };

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