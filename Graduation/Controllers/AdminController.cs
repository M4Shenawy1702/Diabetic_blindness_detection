using CheckEyePro.Core.Interfaces;
using CheckEyePro.Core.Dtos.Auth;
using CheckEyePro.Core.Models;
using CheckEyePro.EF.DBContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Core.IRepository;

namespace CheckEyePro.Api.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[Controller]")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthInterface _authService;
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(ApplicationDbContext context,
                               IAuthInterface authService,
                               IUnitOfWork unitOfWork)
        {
            _context = context;
            _authService = authService;
            _unitOfWork = unitOfWork;
        }
        [HttpPost("AddDoctor")]
        public async Task<IActionResult> AddDoctor([FromForm] RegistrationDto model)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model, "Doctor");
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpPost("AddAdmin")]
        public async Task<IActionResult> AddAdmin([FromForm] RegistrationDto model)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model, "Admin");
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
        //[HttpPost("addRole")]
        //public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var result = await _authService.AddRoleAsync(model);

        //    if (!string.IsNullOrEmpty(result))
        //        return BadRequest(result);
        //    return Ok(model);
        //}
        [HttpDelete("DeleteUser/{UserId}")]
        public async Task<IActionResult> DeletePatient(string UserId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.Delete(UserId);

            return Ok(result);
        }
        [HttpGet("GetAllDoctorsByAdmin")]
        public async Task<IActionResult> GetAllDoctorsByAdmin()
        {
            var Doctors = await _unitOfWork.Doctors.GetAllOrderedDoctors();
            return Ok(Doctors);
        }
        [HttpGet("GetAllPatientsByAdmin")]
        public async Task<IActionResult> GetAllPatientsByAdmin()
        {
            var Patients = await _unitOfWork.Patients.GetAllOrderedPatients();
            return Ok(Patients);
        } 
        [HttpGet("GetAllAdminsByAdmin")]
        public async Task<IActionResult> GetAllAdminsByAdmin()
        {
            var Patients = await _unitOfWork.Admins.GetAllOrderedAdmins();
            return Ok(Patients);
        }
    }
}
