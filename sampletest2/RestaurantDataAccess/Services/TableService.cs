using RestaurantBusiness.Entities;
using RestaurantDataAccess.ServiceContracts;
using RestaurantRepositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantDataAccess.Services
{
    public class TableService : ITableService
    {
        private readonly ITableRepository _tableRepository;
        public TableService(ITableRepository tableRepository)
        {
            _tableRepository = tableRepository;
        }
        public async Task<Table> AddAsync(Table table)
        {
            return await _tableRepository.AddAsync(table);
        }

        public async Task DeleteAsync(int id)
        {
            var listReservation = await _tableRepository.GetTableReservationsAsync(id);
            if (listReservation.Count() > 0)
            {
                throw new InvalidOperationException("The table has reservations and cannot be deleted.");
            }
            await _tableRepository.DeleteAsync(id);
        }

        public Task<IEnumerable<Table>> GetAllAsync()
        {
            return _tableRepository.GetAllAsync();
        }

        public async Task<Table> GetAsync(int id)
        {
            return await _tableRepository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Table table)
        {
            await _tableRepository.UpdateAsync(table);
        }

    }
}
