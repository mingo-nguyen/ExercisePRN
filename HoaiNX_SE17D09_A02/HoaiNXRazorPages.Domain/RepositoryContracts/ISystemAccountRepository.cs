using HoaiNXRazorPages.Domain.Entities;


namespace HoaiNXRazorPages.Domain.RepositoryContracts;
public interface ISystemAccountRepository
{
    Task<IEnumerable<SystemAccount>> GetSystemAccountsAsync();
    Task<SystemAccount> GetSystemAccountByEmailAsync(string email);
    Task AddSystemAccountAsync(SystemAccount systemAccount);
    Task UpdateSystemAccountAsync(SystemAccount systemAccount);
    Task DeleteSystemAccountAsync(SystemAccount systemAccount);

    Task<SystemAccount> GetAccountByIdAsync(short id);
}
