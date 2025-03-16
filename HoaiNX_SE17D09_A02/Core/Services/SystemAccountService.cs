using HoaiNXRazorPages.Application.ServiceContracts;
using HoaiNXRazorPages.Domain.Entities;
using HoaiNXRazorPages.Domain.RepositoryContracts;
using System.Security.Principal;


namespace HoaiNXRazorPages.Core.Services
{
    public class SystemAccountService : ISystemAccountService
    {

        private readonly ISystemAccountRepository _systemAccountRepository;

        public SystemAccountService(ISystemAccountRepository systemAccountRepository)
        {
            _systemAccountRepository = systemAccountRepository;
        }

        public async Task AddAccountAsync(SystemAccount account)
        {
            if (await _systemAccountRepository.GetSystemAccountByEmailAsync(account.AccountEmail) != null)
            {
                throw new ArgumentException("Email already exists");
            }
            var accounts = await _systemAccountRepository.GetSystemAccountsAsync();
            short maxId = accounts.Any() ? accounts.Max(a => a.AccountId) : (short)0;
            account.AccountId = (short)(maxId + 1);
            await _systemAccountRepository.AddSystemAccountAsync(account);
        }

        public async Task DeleteAccountAsync(SystemAccount account)
        {
            await _systemAccountRepository.DeleteSystemAccountAsync(account);
        }

        public async Task<SystemAccount> GetAccountByEmailAsync(string email)
        {
            return await _systemAccountRepository.GetSystemAccountByEmailAsync(email);
        }

        public async Task<SystemAccount> GetAccountByIdAsync(short id)
        {
            return await _systemAccountRepository.GetAccountByIdAsync(id);
        }

        public async Task<IEnumerable<SystemAccount>> GetAccountsAsync()
        {
            return await _systemAccountRepository.GetSystemAccountsAsync();
        }

        public Task<bool> IsAccountExistsAsync(string username)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAccountAsync(SystemAccount account)
        {
            var currentAccount = await GetAccountByIdAsync(account.AccountId);
            string passWord = currentAccount.AccountPassword;
            if (String.IsNullOrEmpty(account.AccountPassword))
            {
                account.AccountPassword = passWord;
            }
            await _systemAccountRepository.UpdateSystemAccountAsync(account);
        }
    }
}
