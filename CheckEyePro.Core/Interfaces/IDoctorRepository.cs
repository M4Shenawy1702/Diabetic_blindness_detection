using CheckEyePro.Core.Dtos;
using CheckEyePro.Core.Dtos.Auth;
using CheckEyePro.Core.Dtos.DoctorDtos;
using CheckEyePro.Core.Models;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Core.IRepository;

namespace CheckEyePro.Core.Interfaces
{
    public interface IDoctorRepository : IGenericRepository<Doctor>
    {
        Task<IEnumerable<Doctor>> GetAllOrderedDoctors();
        Task<Doctor> AddDoctorAsync(ApplicationUser user, RegistrationDto model);
        Task<DoctorProfleDto> ShowProfile(string UserId);
        Task<IEnumerable<ShowObservations>> ShowObservationsByDoctor(string doctorId);
        Task<IEnumerable<HistoryDto>> ShowHistory(string DoctorId);
        Task<string> DeleteHistory(int HistoryId);
        Task<Observation> DoRepot(ReportDto dto, int observationId);
        Task<Observation> AcceptRequest(AcceptRequestDto Dto,int observationId);
        Task<string> ShowFeedback(int observationId);
    }
}
