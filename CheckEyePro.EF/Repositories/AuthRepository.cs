using CheckEyePro.Core.Dtos;
using CheckEyePro.Core.Dtos.Auth;
using CheckEyePro.Core.Errors;
using CheckEyePro.Core.Interfaces;
using CheckEyePro.Core.IServices;
using CheckEyePro.Core.Models;
using CheckEyePro.Core.Settingse;
using CheckEyePro.EF.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderManagementSystem.Core.IRepository;
using System.IdentityModel.Tokens.Jwt;

namespace CheckEyePro.EF.Repositories
{
    public class AuthRepository : IAuthInterface
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJWTTokenGenerator _jWTTokenGenerator;  
        private List<string> _AllowedExtensions = new List<string> { ".jpg", ".png" };
        private long _MaxAllowedSize = 10485760;
        private readonly JWT _jwt;

        public AuthRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IOptions<JWT> jwt, ApplicationDbContext context,
            IJWTTokenGenerator jWTTokenGenerator,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _context = context;
            _jWTTokenGenerator = jWTTokenGenerator;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthModel> RegisterAsync(RegistrationDto model, string Role)
        {

            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered!" };

            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel { Message = "Username is already registered!" };

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
            };
            var result = await _userManager.CreateAsync(user, model.Password);


            using var dataStream = new MemoryStream();
            await model.ProfileImg.CopyToAsync(dataStream);

            var extension = Path.GetExtension(model.ProfileImg.FileName);

            if (!_AllowedExtensions.Contains(extension.ToLower()))
                return new AuthModel { Message = "only .jpg and .png img are allowed" };
            if (model.ProfileImg.Length > _MaxAllowedSize)
                return new AuthModel { Message = "Max Allowed Size is 10Mb" };

            if (result.Succeeded)
            {
                if (Role == "User") await _unitOfWork.Patients.AddPatientAsync(user, model);

                else if (Role == "Doctor") await _unitOfWork.Doctors.AddDoctorAsync(user,model);

                else if (Role == "Admin")await _unitOfWork.Admins.AddAdminAsync(user, model);

            }
            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new AuthModel { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, Role);

            var jwtSecurityToken = await _jWTTokenGenerator.CreateJwtTokenAsync(user);


            return new AuthModel
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { Role },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName,

            };
        }
        public async Task<ApplicationUser> UpdateInfo(string id, UpdateInfoDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new NotFoundException("Patient Not Found");

            if (dto.Email != user.Email)
                if (await _userManager.FindByEmailAsync(dto.Email) is not null)
                    throw new ServiceException(StatusCodes.Status406NotAcceptable, "Email Already Exists");

            using var dataStream = new MemoryStream();
            await dto.ProfileImg.CopyToAsync(dataStream);

            if (await _context.Patients.FirstOrDefaultAsync(x => x.UserId == id)  is Patient patient)
            {
                patient.Username = dto.Username;
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.Email = dto.Email;
                user.UserName = dto.Username;
                user.PhoneNumber = dto.PhoneNumber;

                patient.ProfileImg = dataStream.ToArray();
                if (dto.ProfileImg is not null)
                {
                    var extension = Path.GetExtension(dto.ProfileImg.FileName);
                    if (!_AllowedExtensions.Contains(extension.ToLower()))
                        throw new ServiceException(StatusCodes.Status406NotAcceptable, "only .jpg and .png img are allowed");
                    if (dto.ProfileImg.Length > _MaxAllowedSize)
                        throw new ServiceException(StatusCodes.Status406NotAcceptable, "Max Allowed Size is 10Mb");

                    patient.ProfileImg = dataStream.ToArray();

                }
            } 
            if (await _context.Doctors.FirstOrDefaultAsync(x => x.UserId == id) is Doctor doctor)
            {
                doctor.Username = dto.Username;
                doctor.CareerInfo = dto.CareerInfo;
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.Email = dto.Email;
                user.UserName = dto.Username;
                user.PhoneNumber = dto.PhoneNumber;

                doctor.ProfileImg = dataStream.ToArray();
                if (dto.ProfileImg is not null)
                {
                    var extension = Path.GetExtension(dto.ProfileImg.FileName);
                    if (!_AllowedExtensions.Contains(extension.ToLower()))
                        throw new ServiceException(StatusCodes.Status406NotAcceptable, "only .jpg and .png img are allowed");
                    if (dto.ProfileImg.Length > _MaxAllowedSize)
                        throw new ServiceException(StatusCodes.Status406NotAcceptable, "Max Allowed Size is 10Mb");

                    doctor.ProfileImg = dataStream.ToArray();

                }
            }

            await _context.SaveChangesAsync();

            return user;
        }
        public async Task<ApplicationUser> ChangePassword(string id, string TheNewPass)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new NotFoundException("User Not Found");

            var RemovePass = await _userManager.RemovePasswordAsync(user);
            if (!RemovePass.Succeeded)
                throw new ServiceException(StatusCodes.Status400BadRequest,$"{RemovePass.Errors}");

            var AddPass = await _userManager.AddPasswordAsync(user, TheNewPass);
            if (!AddPass.Succeeded)
                throw new ServiceException(StatusCodes.Status400BadRequest,$"{AddPass.Errors}");

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new ServiceException(StatusCodes.Status400BadRequest,$"{updateResult.Errors}");

            return user;
        }
        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await _jWTTokenGenerator.CreateJwtTokenAsync(user); ;
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();


            return authModel;
        }
        public async Task<AuthModel> Delete(string UserId)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByIdAsync(UserId);
            if (user is null)
            {
                authModel.Message = "User Not Found!";
                return authModel;
            }
            var PatientObservations = await _context.Observations
                .Where(x => x.PId == UserId)
                 .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .ToListAsync();

            var DoctorObservations = await _context.Observations.Where(x => x.DId == UserId).ToListAsync();
 
            var NumDoctorObservation = await _context.Observations.Where(x => x.DId == UserId && x.Status == true).CountAsync();


            if (NumDoctorObservation >= 1)
            {
                authModel.Message = " Doctor has a current obervations You can not delete him until he finishes all his observations";
                return authModel;
            }
            else
            {
                _context.Observations.RemoveRange(DoctorObservations);
                await _context.SaveChangesAsync();
            }

            if(PatientObservations is not null)
            {
                var historyObservations = PatientObservations
                    .Select(O => new History
                {
                    DId = O.DId,
                    Age = O.Age,
                    CreatedOn = O.CreatedOn,
                    Diagnosis = O.Diagnosis,
                    DoctorName = O.Doctor.Username,
                    PatientName = O.Patient.Username,
                    Feedback = O.Feedback,
                    FinishedOn = O.FinishedOn,
                    Gender = O.Gender,
                    MedicalRecord = O.MedicalRecord,
                    MedicalRedation = O.MedicalRedation,
                    PaymentStatus = O.PaymentStatus,
                    PId = O.PId,
                    Report = O.Report,
                    Status = O.Status 
                }).ToList();

                await _context.Histories.AddRangeAsync(historyObservations);

                  _context.Observations.RemoveRange(PatientObservations);
                await _context.SaveChangesAsync();
            }

            var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    authModel.Message = " Failed to delete the user";
                }

                authModel.Message = "User deleted successfully";

            return new AuthModel
            {
                Email = user.Email,
                Username = user.UserName,
                Message = authModel.Message
            };
        }
    }
}