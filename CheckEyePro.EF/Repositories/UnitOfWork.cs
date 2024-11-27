using CheckEyePro.Core.Interfaces;
using CheckEyePro.Core.IServices;
using CheckEyePro.Core.Models;
using CheckEyePro.EF.DBContext;
using CheckEyePro.EF.Repositories;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using OrderManagementSystem.Core.IRepository;

namespace OrderManagementSystem.EF.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public IPatientRepository Patients { get; private set; }
        public IDoctorRepository Doctors { get; private set; }
        public IAdminRepository Admins { get; private set; }
        public IGenericRepository<Payment> Payments { get; private set; }
        public IGenericRepository<Observation> Observations { get; private set; }

        public UnitOfWork(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            Doctors = new DoctorRepository(_context, _userManager, _mapper,_emailService);
            Patients = new PatientRepository(_context, _userManager);
            Admins = new AdminRepository(_context);
            Observations = new GenericRepository<Observation>(_context);
            Payments = new GenericRepository<Payment>(_context);
            _emailService = emailService;
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }
        public void Dispose()
        {
             _context.Dispose();
        }
    }
}
