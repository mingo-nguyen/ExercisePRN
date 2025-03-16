using HoaiNXRazorPages.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoaiNXRazorPages.Application.ServiceContracts
{
    public interface IAuthenticationUserService
    {
        Task<bool> IsAuthenticatedAsync(string email, string password);
        Task<SystemAccount> AuthenticateUserAsync(string email, string password);



    }
}
