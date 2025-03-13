using FUBusiness.Entities;
using FURepositories.Interfaces;
using System.Security.Cryptography;
using System;
using System.Linq;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace FUDataAccess.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;

        public AuthService(IUserRepository userRepository, ISessionRepository sessionRepository)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if(user == null)
            {
                return null;
            }
            if(VerifyPassword(password, user.Password))
            {
                return user;
            }
            return null;
        }

        public async Task<Sessions> CreateSessionAsync(int userId, string role)
        {
            await _sessionRepository.DeleteByUserIdAsync(userId);
            var session = new Sessions
            {
                SessionId = Guid.NewGuid().ToString(),
                UserId = userId,
                Role = role,
                ExpiresAt = DateTime.Now.AddHours(2)
            };
            return await _sessionRepository.AddAsync(session);
        }


        public async Task<bool> ValidateSessionAsync(string sessionId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            return session != null && session.ExpiresAt > DateTime.Now;

        }

        public async Task EndSessionAsync(string sessionId)
        {
             await _sessionRepository.DeleteAsync(sessionId);
        }


        public bool VerifyPassword(string inputPassword, string storedPassword)
        {
            var hash = HashPassword(inputPassword);
            return storedPassword == hash;
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

    }
}
