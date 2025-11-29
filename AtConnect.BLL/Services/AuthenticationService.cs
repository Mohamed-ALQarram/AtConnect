using AtConnect.BLL.DTOs;
using AtConnect.BLL.Helper;
using AtConnect.BLL.Interfaces;
using AtConnect.BLL.Options;
using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AtConnect.BLL.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailService emailService;
        private readonly IConfiguration configuration;
        private readonly JwtOptions jwtOptions;

        public AuthenticationService(IUnitOfWork unitOfWork, IEmailService emailService, IConfiguration configuration, IOptions<JwtOptions> JwtOptions)
        {
            this.unitOfWork = unitOfWork;
            this.emailService = emailService;
            this.configuration = configuration;
            jwtOptions = JwtOptions.Value;
        }
        public async Task<bool> ForgetPasswordAsync(ForgotPasswordDTO forgotPasswordDTO)
        {
            if (string.IsNullOrWhiteSpace(forgotPasswordDTO.Email)) return false;

            var user = await unitOfWork.Users.GetByUserNameOrEmailAsync(forgotPasswordDTO.Email);
            // Always return true to hide if user exists to avoids enumeration attack
            if (user == null)
            {
                await Task.Delay(500);
                return true;
            }

            var otp = OtpGenerator.GenerateSecureOtp();
            user.SetPasswordResetToken(otp, DateTime.UtcNow.AddMinutes(10));
            await unitOfWork.SaveChangesAsync();
            try
            {
                await emailService.SendEmailAsync(new SendEmailDTO(forgotPasswordDTO.Email, EmailTemplates.OtpSubject, EmailTemplates.GetOtpBody(otp)));
            }
            catch (Exception ex)
            {
                // log error
            }

            return true;
        }


        public Task<AuthResponse?> GetUserProfileAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthResponse> LoginAsync(string UserNameOrEmail, string password)
        {
            var User = await unitOfWork.Users.GetByUserNameOrEmailAsync(UserNameOrEmail);
            if (User == null || !PasswordHasher.Verify(password, User.PasswordHash)) throw new InvalidDataException("Invalid user name or password.");
            var token = TokenHelper.CreateJWT(User, configuration, jwtOptions);

            return new AuthResponse
            {
                AccessToken = token,
                ExpiresIn = DateTime.UtcNow.Add(jwtOptions.LifeTime)
            };

        }

        public Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthResponse> RegisterAsync(AppUser user)
        {
            
            if (await unitOfWork.Users.CheckEmailAsync(user.Email)) throw new DuplicateWaitObjectException("This email already exists.");
            if (await unitOfWork.Users.CheckUserNameAsync(user.UserName)) throw new DuplicateWaitObjectException("This UserName already exists.");
            user.ChangePassword(PasswordHasher.Hash(user.PasswordHash));
            await unitOfWork.Users.AddAsync(user);
            await unitOfWork.SaveChangesAsync();
            var token = TokenHelper.CreateJWT(user, configuration, jwtOptions);

            return new AuthResponse
            {
                AccessToken = token,
                ExpiresIn = DateTime.UtcNow.Add(jwtOptions.LifeTime)
            };

        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            if (string.IsNullOrWhiteSpace(resetPasswordDTO.Email)) return false;
            var User = await unitOfWork.Users.GetByUserNameOrEmailAsync(resetPasswordDTO.Email);
            if (User == null) return false;
            if (!VerifyResetPasswordToken(new VerifyResetTokenRequest(resetPasswordDTO.Token, User.PasswordResetToken!, User.ResetTokenExpires))) return false;
            var HashedPassword= PasswordHasher.Hash(resetPasswordDTO.NewPassword);
            User.ChangePassword(HashedPassword);
            await unitOfWork.SaveChangesAsync();
            return true;
        }

        public bool VerifyResetPasswordToken(VerifyResetTokenRequest TokenRequest)
        {
            return TokenRequest.RequestToken == TokenRequest.StoredToken && TokenRequest.ExpireDate>= DateTime.UtcNow;
        }

        public async Task<bool> VerifyResetPasswordTokenAsync(VerifyResetTokenDTO verifyResetTokenDTO)
        {
            if (string.IsNullOrWhiteSpace(verifyResetTokenDTO.Email)) return false;
            var User= await unitOfWork.Users.GetByUserNameOrEmailAsync(verifyResetTokenDTO.Email);
            if (User == null) return false;

            return VerifyResetPasswordToken(new VerifyResetTokenRequest(verifyResetTokenDTO.Token, User.PasswordResetToken!, User.ResetTokenExpires));

        }
    }
}
