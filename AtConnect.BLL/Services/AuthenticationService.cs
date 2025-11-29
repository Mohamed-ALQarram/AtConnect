using AtConnect.BLL.DTOs;
using AtConnect.BLL.Helper;
using AtConnect.BLL.Interfaces;
using AtConnect.BLL.Options;
using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AtConnect.BLL.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailService emailService;
        private readonly IConfiguration configuration;
        private readonly TokenValidationParameters validationParameters;
        private readonly JwtOptions jwtOptions;

        public AuthenticationService(IUnitOfWork unitOfWork, IEmailService emailService,
            IConfiguration configuration, IOptions<JwtOptions> JwtOptions, TokenValidationParameters validationParameters)
        {
            this.unitOfWork = unitOfWork;
            this.emailService = emailService;
            this.configuration = configuration;
            this.validationParameters = validationParameters;
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
                JwtExpiresIn = DateTime.UtcNow.Add(jwtOptions.JwtLifeTime),
                RefreshToken = User.RefreshToken??"",
                RefreshTokenExpiresIn= User.RefreshTokenExpiryTime?? DateTime.UtcNow
            };

        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenDTO refreshTokenDTO)
        {
            var Principal= GetPrincipalFromExpiredToken(refreshTokenDTO.OldToken);
            int.TryParse(Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId);
            if(userId ==0) throw new SecurityTokenException("Invalid token");

            var user = await unitOfWork.Users.GetByKeysAsync(userId);
            if (user == null || user.RefreshToken!=refreshTokenDTO.RefreshToken ||user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null!;
            var newAccessToken = TokenHelper.CreateJWT(user, configuration, jwtOptions);
            var newRefreshToken = GenerateRawRefreshToken();
            var RefreshTokenExpireDate = DateTime.UtcNow.AddDays(jwtOptions.RefreshTokenLifeTime);
            user.SetRefreshToken(newRefreshToken, RefreshTokenExpireDate);
            await unitOfWork.SaveChangesAsync();
            return new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                TokenType = "Bearer",
                JwtExpiresIn =DateTime.UtcNow.Add(jwtOptions.JwtLifeTime) ,
                RefreshTokenExpiresIn = RefreshTokenExpireDate
            };
        }
        private string GenerateRawRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return $"{Convert.ToBase64String(randomNumber)}_{Guid.NewGuid()}";
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken)
                throw new SecurityTokenException("Invalid token");

            return principal;
        }


        public async Task<AuthResponse> RegisterAsync(AppUser User)
        {
            
            if (await unitOfWork.Users.CheckEmailAsync(User.Email)) throw new DuplicateWaitObjectException("This email already exists.");
            if (await unitOfWork.Users.CheckUserNameAsync(User.UserName)) throw new DuplicateWaitObjectException("This UserName already exists.");
            User.ChangePassword(PasswordHasher.Hash(User.PasswordHash));
            var refreshToken = GenerateRawRefreshToken();
            var refreshTokenExpiresIn = DateTime.UtcNow.AddDays(jwtOptions.RefreshTokenLifeTime);
            User.SetRefreshToken(refreshToken, refreshTokenExpiresIn);
            await unitOfWork.Users.AddAsync(User);
            await unitOfWork.SaveChangesAsync();
            var token = TokenHelper.CreateJWT(User, configuration, jwtOptions);

            return new AuthResponse
            {
                AccessToken = token,
                JwtExpiresIn = DateTime.UtcNow.Add(jwtOptions.JwtLifeTime),
                RefreshToken = refreshToken,
                RefreshTokenExpiresIn = refreshTokenExpiresIn
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
