using CheckEyePro.Core.Dtos;
using CheckEyePro.Core.Dtos.Auth;
using CheckEyePro.Core.Dtos.PatientDtos;
using CheckEyePro.Core.Errors;
using CheckEyePro.Core.Interfaces;
using CheckEyePro.Core.Models;
using CheckEyePro.EF.DBContext;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Core.IRepository;
using OrderManagementSystem.EF.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CheckEyePro.EF.Repositories
{
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public PatientRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(context)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IEnumerable<Patient>> GetAllOrderedPatients()
        {
            return await _context.Patients.OrderBy(x => x.Username).ToListAsync();
        }
        public async Task<Patient> AddPatientAsync(ApplicationUser user, RegistrationDto model)
        {
            using var dataStream = new MemoryStream();
            await model.ProfileImg.CopyToAsync(dataStream);

            var patient = new Patient
            {
                UserId = user.Id,
                Username = model.Username,
                ProfileImg = dataStream.ToArray()
            };
            await _context.AddAsync(patient);
            await _context.SaveChangesAsync();

            return patient;
        }

        public async Task<IEnumerable<GetAllDoctorsDto>> GetAllDoctors()
        {
            var Doctors = await _context.Doctors
                     .Include(a => a.User)
                     .Select(a => new GetAllDoctorsDto
                     {
                         DoctorId = a.DoctorId,
                         UserId = a.UserId,
                         FirstName = a.User.FirstName,
                         LastName = a.User.LastName,
                         Email = a.User.Email,
                         CareerInfo = a.CareerInfo,
                         PhoneNumber = a.User.PhoneNumber,
                         ProfileImg = a.ProfileImg,
                         Username = a.Username,
                     })
                     .ToListAsync();
            return Doctors;
        }

        public async Task<PatientProfileDto> ShowProfile(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null) throw new NotFoundException("User Not Found");
            var patient = await _context.Patients.FirstOrDefaultAsync(x => x.UserId == Id);
            if (patient == null) throw new NotFoundException("Patient Not Found");

            var profile = new PatientProfileDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImg = patient.ProfileImg,
                Username = user.UserName,
            };
            return profile;
        }
        public async Task<IEnumerable<ShowObservations>> ShowObservationsByPatient(string PatientId)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(x => x.UserId == PatientId);
            if (patient == null) throw new NotFoundException("Not Found");

            var Observations = await _context.Observations
                .Where(a => a.PId == patient.UserId && a.Status == true)
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
                    Feedback = a.Feedback,
                    MedicalRecord = a.MedicalRecord,
                })
                .ToListAsync();

            return Observations;
        }

        public async Task<IEnumerable<ShowObservations>> ShowRequestedObservationsByPatient(string PatientId)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(x => x.UserId == PatientId);
            if (patient == null) throw new NotFoundException("Patient Not Found");

            var Observations = await _context.Observations
                .Where(a => a.PId == patient.UserId && a.Status == false)
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
                    PaymentStatus = a.PaymentStatus,
                    DoctorMessage = a.DoctorMessage,
                    MedicalRecord = a.MedicalRecord,
                })
                .ToListAsync();

            return Observations;
        }

        public async Task<Observation> CreateFeedback(string Feedback, int observationId)
        {
            var Observation = await _context.Observations.SingleOrDefaultAsync(x => x.ObservationId == observationId);
            if (Observation == null) throw new NotFoundException("Observation not found.");

            if (Observation.Status == false) throw new ServiceException(StatusCodes.Status406NotAcceptable,"the Doctor Should Accept The Observation First ... ");

            Observation.Feedback = Feedback;

            await _context.SaveChangesAsync();

            return Observation;
        }

        public async Task<IEnumerable<HistoryDto>> GetHistory(string patientId)
        {
            var Histories = await _context.Histories
                .Where(a => (a.PId == patientId))
                .OrderBy(a => a.HistoryId)
                .ProjectToType<HistoryDto>()
                .ToListAsync();

            return Histories;
        }

        public async Task<string> DeleteObservation(int observationId)
        {

            var observation = await _context.Observations.SingleOrDefaultAsync(x => x.ObservationId == observationId);
            if (observation == null) throw new NotFoundException("Observation Not Found");

            if (observation.Status == true)
            {
                if (observation.PaymentStatus == PaymentStatus.Paid )
                {
                    var doctor = await _context.Doctors.SingleOrDefaultAsync(x => x.UserId == observation.DId);
                    if (doctor == null) throw new NotFoundException("Doctor Not Found");
                    var patient = await _context.Patients.SingleOrDefaultAsync(x => x.UserId == observation.PId);
                    if (patient == null) throw new NotFoundException("Patient Not Found");

                    var history = new History
                    {
                        DoctorName = doctor.Username,
                        PatientName = patient.Username,
                        CreatedOn = observation.CreatedOn,
                        FinishedOn = DateTime.Now,
                        Diagnosis = observation.Diagnosis,
                        DId = observation.DId,
                        PId = observation.PId,
                        Report = observation.Report,
                        Feedback = observation.Feedback,
                        MedicalRedation = observation.MedicalRedation,
                        PaymentStatus = PaymentStatus.Paid,
                        Status = observation.Status,
                        Gender = observation.Gender,
                        Age = observation.Age,
                        MedicalRecord = observation.MedicalRecord,
                    };
                    await _context.Histories.AddAsync(history);
                    _context.Observations.Remove(observation);
                    await _context.SaveChangesAsync();
                    return"Observation Deleted Successfully";
                }
                else
                     throw new Exception("You Should Pay First.");
            }
            else
            {
                _context.Observations.Remove(observation);
                await _context.SaveChangesAsync();
                return "Observation Deleted successfully";
            }
        }

        public async Task<string> ShowReport(int observationId)
        {
            var observation = await _context.Observations.SingleOrDefaultAsync(x => x.ObservationId == observationId);
            if (observation == null) throw new NotFoundException("Not Found");

            if (observation.Report != null)
            {
                if (observation.PaymentStatus == PaymentStatus.Paid)
                {
                    var Report = observation.Report;
                    return Report;
                }
                else
                    throw new ServiceException(StatusCodes.Status406NotAcceptable,"You Should Pay First.");
            }
            else
            {
                throw new ServiceException(StatusCodes.Status400BadRequest,"The doctor has not prepared the report yet.");
            }
        }


    }
}
