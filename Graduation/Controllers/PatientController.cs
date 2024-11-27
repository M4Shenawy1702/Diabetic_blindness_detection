using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using CheckEyePro.Core.Dtos;
using CheckEyePro.Core.Dtos.Auth;
using CheckEyePro.Core.Interfaces;
using CheckEyePro.Core.IServices;
using CheckEyePro.Core.Models;
using CheckEyePro.EF.DBContext;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using OrderManagementSystem.Core.IRepository;
using Stripe.Checkout;
using CheckEyePro.Core.Errors;
using Microsoft.CodeAnalysis;
using CheckEyePro.Core.Dtos.PatientDtos;


namespace Graduation.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[Controller]")]
    [ApiController]
    public class PatientController : Controller
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        private List<string> _AllowedExtensions = new List<string> { ".jpg", ".png" };
        private long _MaxAllowedSize = 10485760;
        private readonly IPrediction _prediction;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthInterface _authInterface;
        private readonly HttpClient _httpClient;


        public PatientController(
            IWebHostEnvironment webHostEnvironment,
            IPrediction prediction,
            IUnitOfWork unitOfWork,
            IAuthInterface authInterface)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:8000/predict");
            _httpClient.Timeout = TimeSpan.FromSeconds(500);
            _prediction = prediction;
            _unitOfWork = unitOfWork;
            _authInterface = authInterface;
        }
        [HttpGet("GetAllDoctors")]
        public async Task<IActionResult> GetAllDoctors()=>Ok(await _unitOfWork.Patients.GetAllDoctors());
        
        [HttpGet("ShowProfile/{Id}")]
        public async Task<IActionResult> ShowProfile(string Id) => Ok(await _unitOfWork.Patients.ShowProfile(Id));

        [HttpPut("UpdatePatientInfo/{id}")]
        public async Task<IActionResult> UpdatePatientInfo(string id, [FromForm] UpdateInfoDto dto)
        {
            return Ok(await _authInterface.UpdateInfo(id, dto));
        }
        [HttpPut("ChangePassword/{Patient_Id}")]
        public async Task<IActionResult> ChangePassword (string Patient_Id , ChangePassDto Dto)
        {
            return Ok(await _authInterface.ChangePassword(Patient_Id, Dto.TheNewPass));
        }

        [HttpGet("ShowObservationsByPatient/{PatientId}")]
        public async Task<IActionResult> ShowObservationsByPatient(string PatientId)
        {
            return Ok(await _unitOfWork.Patients.ShowObservationsByPatient(PatientId));
        }
        [HttpGet("ShowRequestedObservationsByPatient/{PatientId}")]
        public async Task<IActionResult> ShowRequestedObservationsByPatient(string PatientId)
        {
            return Ok(await _unitOfWork.Patients.ShowRequestedObservationsByPatient(PatientId));
        }

        [HttpPost("AskOnlineObservation/{patientId}")]
        public async Task<IActionResult> AskOnlineObservation(string patientId, [FromForm] ObservationDto dto)
        {
            var patient = await _unitOfWork.Patients.FindAsync(x => x.UserId == patientId);
            if (patient == null) return BadRequest("Patient Not Found");

            var Doctor = await _unitOfWork.Doctors.FindAsync(x => x.UserId == dto.DId);
            if (Doctor == null) return BadRequest("Doctor Not Found");

            var validationPassed = ValidateObservationDto(dto, out var errorMessage);
            if (!validationPassed) return BadRequest(errorMessage);

            var CheckObservation = await _unitOfWork.Observations.FindAsync(u => u.PId == patientId && u.DId == dto.DId);

            if (CheckObservation is null)
            {
                string Diagnosis;

                var doctorObservations = await _unitOfWork.Observations
                    .FindAllWithcraiteriaAsync(a => a.DId == Doctor.UserId && a.Status == true);

                if (doctorObservations.Count() >= 30)
                    return Ok("The doctor has reached the maximum number of patients.");
                else
                {
                    var observation = new Observation
                    {
                        DId = dto.DId,
                        Status = false,
                        PId = patientId,
                        CreatedOn = DateTime.Now,
                        Age = dto.Age,
                        Gender = dto.Gender,
                        MedicalRecord = dto.MedicalRecord,
                    };
                    await _unitOfWork.Observations.AddAsync(observation);
                   _unitOfWork.Complete();
                    var ObservationID = observation.ObservationId.ToString();
                    if (dto.MedicalRedation is not null)
                    {
                        using var dataStream = new MemoryStream();
                        await dto.MedicalRedation.CopyToAsync(dataStream);


                        var extension = Path.GetExtension(dto.MedicalRedation.FileName);
                        var imageName = $"{ObservationID}.png";


                        if (!_AllowedExtensions.Contains(extension.ToLower()))
                            return BadRequest("only .jpg and .png img are allowed");
                        if (dto.MedicalRedation.Length > _MaxAllowedSize)
                            return BadRequest("Max Allowed Size is 10Mb");

                        try
                        {
                            Diagnosis = await _prediction.PredectApi(dto.MedicalRedation);
                        }
                        catch (Exception)
                        {
                            throw new ServiceException(StatusCodes.Status500InternalServerError, "Something went wrong while predection");
                        }


                        if (Diagnosis == "0")
                        {
                            var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/Images/MedicalRedations/0", imageName);
                            using var stream = System.IO.File.Create(path);
                            dto.MedicalRedation.CopyTo(stream);
                        }
                        if (Diagnosis == "1")
                        {
                            var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/Images/MedicalRedations/1", imageName);
                            using var stream = System.IO.File.Create(path);
                            dto.MedicalRedation.CopyTo(stream);
                        }
                        if (Diagnosis == "2")
                        {

                            var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/Images/MedicalRedations/2", imageName);
                            using var stream = System.IO.File.Create(path);
                            dto.MedicalRedation.CopyTo(stream);
                        }
                        if (Diagnosis == "3")
                        {

                            var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/Images/MedicalRedations/3", imageName);
                            using var stream = System.IO.File.Create(path);
                            dto.MedicalRedation.CopyTo(stream);
                        }
                        if (Diagnosis == "4")
                        {
                            var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/Images/MedicalRedations/4", imageName);
                            using var stream = System.IO.File.Create(path);
                            dto.MedicalRedation.CopyTo(stream);
                        }

                        observation.MedicalRedation = dataStream.ToArray();
                        observation.Diagnosis = Diagnosis;
                    }

                     _unitOfWork.Complete();

                    return Ok(observation);
                }
            }
            else return Ok("Observation already eaxists");
        }
        [HttpPut("Feedback/{observationId}")]
        public async Task<IActionResult> Feedback([FromForm]  FeedbackDto dto ,int observationId)
        {
            return Ok(await _unitOfWork.Patients.CreateFeedback(dto.Feedback, observationId));
        }
        [HttpGet("History/{patientId}")]
        public async Task<IActionResult> GetHistory(string patientId)
        {
            return Ok(await _unitOfWork.Patients.GetHistory(patientId));
        }
        [HttpGet("CreateFeedback/{observationId}")]
        public async Task<IActionResult> CreateFeedback([FromForm] FeedbackDto dto, int observationId)
        {
            return Ok(await _unitOfWork.Patients.CreateFeedback(dto.Feedback,observationId));
        }
        [HttpGet("ShowReport/{observationId}")]
        public async Task<IActionResult> ShowReport(int observationId)
        {
            return Ok(await _unitOfWork.Patients.ShowReport(observationId));
        }

        [HttpPost("CreatePayment/{ObservationId}")]
        public async Task<IActionResult> CreatePayment([FromServices] IServiceProvider sp, int ObservationId)
        {
            var Observation = await _unitOfWork.Observations
                .GetByIdAsync(ObservationId);
            if (Observation == null) return NotFound("Observation not found.");


            if (Observation.PaymentStatus == PaymentStatus.Paid) return BadRequest("The Observation has already been paid.");
            else
            {
                if (Observation.Status == false) return BadRequest("the Doctor should accept it first to do payment");

                else
                {
                    // Build the URL to which the customer will be redirected after paying.
                    var server = sp.GetRequiredService<IServer>();

                    var serverAddressesFeature = server.Features.Get<IServerAddressesFeature>();

                    string? thisApiUrl = null;

                    if (serverAddressesFeature is not null)
                        thisApiUrl = serverAddressesFeature.Addresses.FirstOrDefault();
                    

                    if (thisApiUrl is not null)
                    {
                        var SessionUrl = await CheckOut(thisApiUrl, ObservationId);
                        return Ok(SessionUrl);
                    }
                    else
                        return StatusCode(500);
                }
            }
        }

        [NonAction]
        public async Task<string> CheckOut(string thisApiUrl, int ObservationId)
        {

            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{thisApiUrl}api/Patient/success?sessionId={{CHECKOUT_SESSION_ID}}&observationId={ObservationId}", // Customer paid.
                CancelUrl = $"{thisApiUrl}api/Patient/failed?sessionId={{CHECKOUT_SESSION_ID}}", // Checkout cancelled.
                PaymentMethodTypes = new List<string> { "card", "applepay" },
                LineItems =
                        [ new SessionLineItemOptions
                            {   
                                Price = "price_1PMpjiHo3w5F4xJqCPwyE1l0",
                                Quantity = 1, 
                              
                              },
                         ],

                Mode = "payment",
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Url;
        }

        [HttpGet("success")]
        public async Task<ActionResult> CheckoutSuccessAsync(string sessionId, int ObservationId)
        {
            var Checkpayment = await _unitOfWork.Payments.GetByIdAsync(ObservationId);
            if (Checkpayment is not null) return Ok("Observation has been paid successfully");

            var observation = await _unitOfWork.Observations.GetByIdAsync(ObservationId);
            if (observation is null) return BadRequest("Observation Not Found");

            var sessionService = new SessionService();
            var session = sessionService.Get(sessionId);

            var payment = new Payment
            {
                Date = DateTime.Now,
                ObservationId = ObservationId,
                Amount = 500
            };

            observation.PaymentStatus = PaymentStatus.Paid;
            await _unitOfWork.Payments.AddAsync(payment);

             _unitOfWork.Complete();

            return Ok("Payment Done");
        }

        [HttpGet("failed")]
        public async Task<ActionResult> failed(string sessionId)
        {
            var sessionService = new SessionService();
            var session = sessionService.Get(sessionId);

            return Ok("Payment Canceled");
        }


        private bool ValidateObservationDto(ObservationDto dto, out string? errorMessage)
        {
            errorMessage = null;

            if (dto == null)
            {
                errorMessage = "Observation data is missing.";
                return false;
            }

            if (dto.Age <= 0)
            {
                errorMessage = "Invalid age value.";
                return false;
            }

            return true;
        }

    }
}