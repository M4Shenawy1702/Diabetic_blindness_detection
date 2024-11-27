using CheckEyePro.Core.Dtos.Auth;
using CheckEyePro.Core.Interfaces;
using CheckEyePro.Core.Models;
using CheckEyePro.EF.DBContext;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.EF.Repository;

namespace CheckEyePro.EF.Repositories
{
    internal class AdminRepository : GenericRepository<Admin>, IAdminRepository
    {
        private readonly ApplicationDbContext _context;
        public AdminRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public async  Task<IEnumerable<Admin>> GetAllOrderedAdmins()
        {
            return await _context.Admins.OrderBy(x => x.Username).ToListAsync();
        }
        public async Task<Admin> AddAdminAsync(ApplicationUser user, RegistrationDto model)
        {
            using var dataStream = new MemoryStream();
            await model.ProfileImg.CopyToAsync(dataStream);

            var admin = new Admin
            {
                UserId = user.Id,
                Username = user.UserName,
                ProfileImg = dataStream.ToArray(),
            };
            await _context.AddAsync(admin);
            await _context.SaveChangesAsync();


            return admin;
        }

    }
}
