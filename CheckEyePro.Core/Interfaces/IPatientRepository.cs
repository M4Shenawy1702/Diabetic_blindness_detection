using CheckEyePro.Core.Dtos;
using CheckEyePro.Core.Dtos.Auth;
using CheckEyePro.Core.Dtos.PatientDtos;
using CheckEyePro.Core.Models;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Core.IRepository;

namespace CheckEyePro.Core.Interfaces
{
    public interface IPatientRepository : IGenericRepository<Patient>
    {
        Task<IEnumerable<Patient>> GetAllOrderedPatients();
        Task<IEnumerable<GetAllDoctorsDto>> GetAllDoctors();
        Task<Patient> AddPatientAsync(ApplicationUser user, RegistrationDto model);
        Task<PatientProfileDto> ShowProfile(string Id);
        Task<IEnumerable<ShowObservations>> ShowObservationsByPatient(string PatientId);
        Task<IEnumerable<ShowObservations>> ShowRequestedObservationsByPatient(string PatientId);
        Task<Observation> CreateFeedback(string Feedback, int observationId);
        Task<IEnumerable<HistoryDto>> GetHistory(string patientId);
        Task<string> DeleteObservation(int observationId);
        Task<string> ShowReport(int observationId);
    }
}
