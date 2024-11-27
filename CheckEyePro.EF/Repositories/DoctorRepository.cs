using CheckEyePro.Core.Dtos;
using CheckEyePro.Core.Dtos.Auth;
using CheckEyePro.Core.Dtos.DoctorDtos;
using CheckEyePro.Core.Errors;
using CheckEyePro.Core.Interfaces;
using CheckEyePro.Core.IServices;
using CheckEyePro.Core.Models;
using CheckEyePro.EF.DBContext;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.EF.Repository;

namespace CheckEyePro.EF.Repositories
{
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public DoctorRepository(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IEmailService emailService) : base(context)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _emailService = emailService;
        }
        public async  Task<IEnumerable<Doctor>> GetAllOrderedDoctors()
        {
            return await _context.Doctors.OrderBy(x => x.Username).ToListAsync();
        }
        public async Task<Doctor> AddDoctorAsync(ApplicationUser user, RegistrationDto model)
        {
            using var dataStream = new MemoryStream();
            await model.ProfileImg.CopyToAsync(dataStream);

            var Doctor = new Doctor
            {
                UserId = user.Id,
                Username = user.UserName,
                ProfileImg = dataStream.ToArray(),
                CareerInfo = model.CareerInfo,
            };
            await _context.Doctors.AddAsync(Doctor);
            await _context.SaveChangesAsync();


            return Doctor;
        }

        public async Task<DoctorProfleDto> ShowProfile(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null) throw new NotFoundException("Not Found");
            var doctor = await _context.Doctors.FirstOrDefaultAsync(x => x.UserId == UserId);
            if (doctor == null) throw new NotFoundException("Not Found");

            var profile = new DoctorProfleDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImg = doctor.ProfileImg,
                Username = user.UserName,
                CareerInfo = doctor.CareerInfo,
            };
            return profile;
        }
        public async Task<IEnumerable<ShowObservations>> ShowObservationsByDoctor(string doctorId)
        {
            var Doctor = await _context.Doctors.FirstOrDefaultAsync(x => x.UserId == doctorId);
            if (Doctor == null) throw new NotFoundException("Doctor Not Found");

            var Observations = await _context.Observations
                .Where(a => a.DId == Doctor.UserId)
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Select(a => new ShowObservations
                {
                    ObservationId = a.ObservationId,
                    DoctorName = a.Doctor.Username,
                    PatientName = a.Patient.Username,
                    CreatedOn = a.CreatedOn,
                    Diagnosis = a.Diagnosis,
                    DoctorId = a.Doctor.UserId,
                    PatientId = a.Patient.UserId,
                    MedicalRedation = a.MedicalRedation,
                    Status = a.Status,
                    PaymentStatus = a.PaymentStatus,
                    Report = a.Report,
                    Feedback = a.Feedback,
                    MedicalRecord = a.MedicalRecord,
                })
                .ToListAsync();

            return Observations;
        }

        public async Task<IEnumerable<HistoryDto>> ShowHistory(string DoctorId)
        {

            var Doc = await _context.Histories.SingleOrDefaultAsync(x => x.DId == DoctorId);
            if (Doc == null) throw new NotFoundException("Not Found");

            var histories = await _context.Histories
                .Where(a => a.DId == DoctorId)
                .OrderBy(a => a.HistoryId)
                .ProjectToType<HistoryDto>() // Mapster does the mapping here
                .ToListAsync();

            return histories;
        }

        public async Task<string> DeleteHistory(int HistoryId)
        {
            var History = await _context.Histories.SingleOrDefaultAsync(x => x.HistoryId == HistoryId);
            if (History == null) throw new NotFoundException("Not Found");

            _context.Histories.Remove(History);
            await _context.SaveChangesAsync();

            return "History Deleted successfully";
        }

        public async Task<Observation> DoRepot(ReportDto dto, int observationId)
        {
            var Observation = await _context.Observations.SingleOrDefaultAsync(x => x.ObservationId == observationId);
            if (Observation == null) throw new NotFoundException("Not Found");

            var Patient = await _userManager.FindByIdAsync(Observation.PId);
            if (Patient == null) throw new NotFoundException("Not Found");


            if (Observation.Status == false)
                throw new NotFoundException("You Should Accept The Observation First ... ");
            else
            {
                Observation.Report = dto.Report;
                //Send an Email to Inform the Patient
                await _emailService.SendEmailAsync(Patient.Email, "Report", "Your Report is Done");
            }
               
            await _context.SaveChangesAsync();

            return Observation;
        }

        public async Task<Observation> AcceptRequest(AcceptRequestDto Dto, int observationId)
        {
            var observation = await _context.Observations.SingleOrDefaultAsync(x => x.ObservationId == observationId);
            if (observation == null) throw new NotFoundException("Not Found");

            var Patient = await _userManager.FindByIdAsync(observation.PId);
            if (Patient == null) throw new NotFoundException("Not Found");

            if (Dto.Status == true)
            {   
                observation.Status = true;
                //Send an Email to Inform the Patient
                await _emailService.SendEmailAsync(Patient.Email, "observationRequest", "The Doctor Accept the Observation");
            }
                
            else
            {
                  observation.Status = false;
                observation.DoctorMessage = " The Doctor Reject the Observation Please delete it ";
                //Send an Email to Inform the Patient
                await _emailService.SendEmailAsync(Patient.Email, "observationRequest", "The Doctor Reject the Observation Please delete it ");
            }

            await _context.SaveChangesAsync();

            return observation;
        }

        public async Task<string> ShowFeedback(int observationId)
        {
            var observation = await _context.Observations.SingleOrDefaultAsync(x => x.ObservationId == observationId);
            if (observation == null) throw new NotFoundException("Not Found");

            if (observation.Feedback != null)
                return observation.Feedback;
            else
                return "The Patient has not prepared the Feedback yet.";
        }
    }
}
