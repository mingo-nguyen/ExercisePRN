using FUBusiness.Entities;
using FURepositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FUDataAccess.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<IEnumerable<User>> GetAllStudentsAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Where(u => u.Role == "Student");
        }

        public async Task<User> AddUserAsync(User user)
        {
            // Validate email uniqueness
            if (await _userRepository.EmailExistsAsync(user.Email))
            {
                throw new InvalidOperationException($"A user with email {user.Email} already exists");
            }

            return await _userRepository.AddAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            // Check if email is changing and validate uniqueness
            var existingUser = await _userRepository.GetByIdAsync(user.ID);
            if (existingUser.Email != user.Email && await _userRepository.EmailExistsAsync(user.Email))
            {
                throw new InvalidOperationException($"A user with email {user.Email} already exists");
            }

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }

        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
