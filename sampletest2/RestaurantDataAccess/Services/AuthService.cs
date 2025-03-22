using RestaurantBusiness.Entities;
using RestaurantRepositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantDataAccess.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            var user =  await _userRepository.GetByUserNameAsync(username);
            if (user == null)
            {
                return null;
            }
                
            if(VerifyPassword(password, user.Password))
            {
                return user;
            }
            return null;

        }


        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            var hash = HashPassword(inputPassword);
            return hash == storedPassword;
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
