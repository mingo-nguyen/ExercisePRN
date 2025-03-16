
using HoaiNXRazorPages.Domain.Entities;
using HoaiNXRazorPages.Domain.RepositoryContracts;
using HoaiNXRazorPages.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HoaiNXRazorPages.Infrastructure.Repositories
{
    public class SystemAccountRepository : ISystemAccountRepository
    {

        private readonly MyDbContext _context;

        public SystemAccountRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task AddSystemAccountAsync(SystemAccount systemAccount)
        {
            await _context.SystemAccounts.AddAsync(systemAccount);
            await _context.SaveChangesAsync();  
        }

        public async Task DeleteSystemAccountAsync(SystemAccount systemAccount)
        {
            _context.SystemAccounts.Remove(systemAccount);
            await _context.SaveChangesAsync();
        }

        public async Task<SystemAccount> GetAccountByIdAsync(short id)
        {
            return await _context.SystemAccounts.FindAsync(id);
        }

        public async Task<SystemAccount> GetSystemAccountByEmailAsync(string email)
        {
            return await _context.SystemAccounts.FirstOrDefaultAsync(u => u.AccountEmail == email);
        }

        public async Task<IEnumerable<SystemAccount>> GetSystemAccountsAsync()
        {
            return await _context.SystemAccounts.ToListAsync();
        }

        public async Task UpdateSystemAccountAsync(SystemAccount systemAccount)
        {
            var existingAccount = await _context.SystemAccounts.FindAsync(systemAccount.AccountId);
            _context.Entry(existingAccount).CurrentValues.SetValues(systemAccount);
            await _context.SaveChangesAsync();
        }
    }
}
