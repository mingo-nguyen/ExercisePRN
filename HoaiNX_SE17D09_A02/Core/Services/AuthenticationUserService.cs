using HoaiNXRazorPages.Application.ServiceContracts;
using HoaiNXRazorPages.Domain.Entities;
using HoaiNXRazorPages.Domain.RepositoryContracts;
using Microsoft.Extensions.Configuration;


namespace HoaiNXRazorPages.Core.Services
{
    public class AuthenticationUserService : IAuthenticationUserService
    {
        private readonly ISystemAccountRepository _systemAccountRepository;
        private IConfiguration _configuration;


        public AuthenticationUserService(ISystemAccountRepository systemAccountRepository, IConfiguration configuration)
        {
            _systemAccountRepository = systemAccountRepository;
            _configuration = configuration;
        }

        public async Task<SystemAccount> AuthenticateUserAsync(string email, string password)
        {
            var adminEmail = _configuration["Admin:Email"];
            var adminPassword = _configuration["Admin:Password"];   

            if(email == adminEmail && password == adminPassword)
            {
                return new SystemAccount
                {
                    AccountId = 0,
                    AccountName = "Admin",
                    AccountEmail = adminEmail,
                    AccountRole = 0
                };
            }

            var account = await _systemAccountRepository.GetSystemAccountByEmailAsync(email);
            if (account != null && password == account.AccountPassword)
            {
                return account;
            }
            return null; 

        }

        public async Task<bool> IsAuthenticatedAsync(string email, string password)
        {
            var user = await AuthenticateUserAsync(email, password);
            return user != null;
        }
    }
}
