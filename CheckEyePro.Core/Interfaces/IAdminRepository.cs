using CheckEyePro.Core.Dtos.Auth;
using CheckEyePro.Core.Models;
using OrderManagementSystem.Core.IRepository;

namespace CheckEyePro.Core.Interfaces
{
    public interface IAdminRepository : IGenericRepository<Admin>
    {
        Task<IEnumerable<Admin>> GetAllOrderedAdmins();
        Task<Admin> AddAdminAsync(ApplicationUser user, RegistrationDto model);
    }
}
