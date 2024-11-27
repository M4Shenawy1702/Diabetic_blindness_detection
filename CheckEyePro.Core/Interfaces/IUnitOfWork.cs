using CheckEyePro.Core.Interfaces;
using CheckEyePro.Core.Models;

namespace OrderManagementSystem.Core.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IPatientRepository Patients { get; }
        IDoctorRepository Doctors { get; }
        IAdminRepository Admins { get; }
        IGenericRepository<Payment> Payments { get; }
        IGenericRepository<Observation> Observations { get; }
        int Complete();
    }
}
