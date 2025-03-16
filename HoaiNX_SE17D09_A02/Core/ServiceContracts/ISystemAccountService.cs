
using HoaiNXRazorPages.Domain.Entities;
using System.Threading.Tasks;

namespace HoaiNXRazorPages.Application.ServiceContracts
{
    public interface ISystemAccountService
    {
        Task<IEnumerable<SystemAccount>> GetAccountsAsync();
        Task AddAccountAsync(SystemAccount account);
        Task UpdateAccountAsync(SystemAccount account);
        Task DeleteAccountAsync(SystemAccount account);
        Task<bool> IsAccountExistsAsync(string username);

        Task<SystemAccount> GetAccountByEmailAsync(string email);


        Task<SystemAccount> GetAccountByIdAsync(short id);
    }
}
