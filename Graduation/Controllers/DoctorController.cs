using CheckEyePro.Core.Dtos.Auth;
using CheckEyePro.Core.Dtos.DoctorDtos;
using CheckEyePro.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Core.IRepository;

namespace Graduation.Controllers
{
    [Authorize(Roles = "Doctor")]
    [Route("api/[Controller]")]
    [ApiController]
    public class DoctorController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthInterface _authInterface;


        public DoctorController(
        IUnitOfWork unitOfWork,
        IWebHostEnvironment webHostEnvironment,
        IAuthInterface authInterface)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _authInterface = authInterface;
        }
        [HttpGet("ShowProfile/{UserId}")]
        public async Task<IActionResult> ShowProfile(string UserId)
        {
            return Ok(await _unitOfWork.Doctors.ShowProfile(UserId));
        }
        [HttpPut("UpdateDoctorInfo/{UserId}")]
        public async Task<IActionResult> UpdateDoctorInfo(string UserId, [FromForm] UpdateInfoDto dto)
        {
            return Ok(await _authInterface.UpdateInfo(UserId, dto));
        }
        [HttpPut("ChangePassword/{id}")]
        public async Task<IActionResult> ChangePassword(string Patient_Id, ChangePassDto Dto)
        {
            return Ok(await _authInterface.ChangePassword(Patient_Id, Dto.TheNewPass));
        }
        [HttpGet("ShowObservationsByDoctor/{doctorId}")]
        public async Task<IActionResult> ShowObservationsByDoctor(string doctorId)
        {
            return Ok(await _unitOfWork.Doctors.ShowObservationsByDoctor(doctorId));
        }
      
        [HttpGet("History/{DoctorId}")]
        public async Task<IActionResult> History(string DoctorId)
        {
            return Ok(await _unitOfWork.Doctors.ShowHistory(DoctorId));
        }
        [HttpDelete("DeleteHistory/{HistoryId}")]
        public async Task<IActionResult> DeleteHistory(int HistoryId)
        {
            return Ok( await _unitOfWork.Doctors.DeleteHistory(HistoryId));
        }
        [HttpPut("DoRepot/{observationId}")]
        public async Task<IActionResult> Repot([FromForm] ReportDto dto, int observationId)
        {
            return Ok( await _unitOfWork.Doctors.DoRepot(dto, observationId));
        }
        [HttpPut("AcceptRequest/{observationId}")]
        public async Task<IActionResult> AcceptRequest([FromBody] AcceptRequestDto Dto,int observationId )
        {
            return Ok(await _unitOfWork.Doctors.AcceptRequest(Dto, observationId));
        }
        [HttpGet("ShowFeedback/{observationId}")]
        public async Task<IActionResult> ShowFeedback(int observationId)
        {
            return Ok(await _unitOfWork.Doctors.ShowFeedback(observationId));
        }
        [HttpPut("UpdateDiagnosis/{observationId}")]
        public async Task<IActionResult> UpdateDiagnosis(int observationId, [FromForm] UpdateDiagnosisDto Dto)
        {
            var observation = await _unitOfWork.Observations.GetByIdAsync(observationId);
            if (observation == null) throw new Exception("Not Found");
            if (observation.Diagnosis == Dto.Doctordiagnosis) throw new Exception("This is the current diagnosis!");

            var sourcePath = $"{_webHostEnvironment.WebRootPath}/Images/MedicalRedations/{observation.Diagnosis}/{observation.ObservationId}.png";

            if (Dto.Doctordiagnosis == "0")
            {
                var destinationPath = $"{_webHostEnvironment.WebRootPath}/Images/MedicalRedations/0/{observation.ObservationId}.png";
                System.IO.File.Copy(sourcePath, destinationPath);
                System.IO.File.Delete(sourcePath);
            }
            else if (Dto.Doctordiagnosis == "1")
            {
                var destinationPath = $"{_webHostEnvironment.WebRootPath}/Images/MedicalRedations/1/{observation.ObservationId}.png";
                System.IO.File.Copy(sourcePath, destinationPath);
                System.IO.File.Delete(sourcePath);
            }
            else if (Dto.Doctordiagnosis == "2")
            {
                var destinationPath = $"{_webHostEnvironment.WebRootPath}/Images/MedicalRedations/2/{observation.ObservationId}.png";
                System.IO.File.Copy(sourcePath, destinationPath);
                System.IO.File.Delete(sourcePath);
            }
            else if (Dto.Doctordiagnosis == "3")
            {
                var destinationPath = $"{_webHostEnvironment.WebRootPath}/Images/MedicalRedations/3/{observation.ObservationId}.png";
                System.IO.File.Copy(sourcePath, destinationPath);
                System.IO.File.Delete(sourcePath);
            }
            else if (Dto.Doctordiagnosis == "4")
            {
                var destinationPath = $"{_webHostEnvironment.WebRootPath}/Images/MedicalRedations/4/{observation.ObservationId}.png";
                System.IO.File.Copy(sourcePath, destinationPath);
                System.IO.File.Delete(sourcePath);
            }
            else
            {
                throw new Exception("Wrong diagnosis");
            }
            observation.Diagnosis = Dto.Doctordiagnosis;
            _unitOfWork.Complete();

            return Ok(observation);
        }
    }
}
