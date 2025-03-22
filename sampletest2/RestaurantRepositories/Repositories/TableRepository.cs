using RestaurantBusiness.Data;
using RestaurantBusiness.Entities;
using RestaurantRepositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantRepositories.Repositories
{
    public class TableRepository : ITableRepository
    {
        private readonly RestaurantDbContext _context;
        public TableRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<Table> AddAsync(Table table)
        {
           await _context.Tables.AddAsync(table);
            await _context.SaveChangesAsync();
            return table;
        }

        public async Task DeleteAsync(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table != null)
            {
                _context.Tables.Remove(table);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Table>> GetAllAsync()
        {
            return await _context.Tables.ToListAsync();
        }

        public async Task<Table> GetByIdAsync(int id)
        {
            return await _context.Tables.FindAsync(id);
        }

        public async Task UpdateAsync(Table table)
        {
            var existingTable = await _context.Tables.FindAsync(table.Id);

            if (existingTable != null)
            {
                _context.Entry(existingTable).CurrentValues.SetValues(table);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<IEnumerable<Table>> SearchTablesAsync(int seats, string status = null)
        {
            var query = _context.Tables.AsQueryable();

            if (seats > 0)
            {
                query = query.Where(t => t.Seats == seats);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetTableReservationsAsync(int tableId)
        {
            return await _context.Reservations
                .Where(r => r.TableId == tableId)
                .ToListAsync();
        }
    }
}
