using RestaurantBusiness.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantRepositories.Interfaces
{
    public interface ITableRepository
    {
        public Task<Table> GetByIdAsync(int id);
        public Task<IEnumerable<Table>> GetAllAsync();
        public Task<Table> AddAsync(Table table);
        public Task UpdateAsync(Table table);
        public Task DeleteAsync(int id);
        public Task<IEnumerable<Reservation>> GetTableReservationsAsync(int tableId);
    }
}
