using RestaurantBusiness.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantDataAccess.ServiceContracts
{
    public  interface ITableService
    {
        Task<Table> GetAsync(int id);       
        Task<IEnumerable<Table>> GetAllAsync();

        Task<Table> AddAsync(Table table);
        Task UpdateAsync(Table table);
        Task DeleteAsync(int id);


    }
}
