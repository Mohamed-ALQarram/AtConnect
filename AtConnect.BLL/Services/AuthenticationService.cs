using AtConnect.BLL.DTOs;
using AtConnect.BLL.Helper;
using AtConnect.BLL.Interfaces;
using AtConnect.BLL.Options;
using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.DTOs;
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

        public async Task<bool> RegisterAsync(AppUser User)
        {
            if (await unitOfWork.Users.CheckEmailAsync(User.Email)) throw new DuplicateWaitObjectException("This email already exists.");
            if (await unitOfWork.Users.CheckUserNameAsync(User.UserName)) throw new DuplicateWaitObjectException("This UserName already exists.");

            User.ChangePassword(PasswordHasher.Hash(User.PasswordHash));
            User.EmailVerification(false);

            await SendTokenToEmailAsync(User);
            await unitOfWork.Users.AddAsync(User);
            await unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<TokenDTO> VerifyEmailToken(ConfirmEmailVerificationRequest verificationToken)
        {
            var User=  await VerifyTokenAsync(verificationToken);
            if (User == null) return null!;
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiresIn = DateTime.UtcNow.AddDays(jwtOptions.RefreshTokenLifeTime);
            User.EmailVerification(true);
            User.ClearVerifyToken();
            User.SetRefreshToken(refreshToken, refreshTokenExpiresIn);
            await unitOfWork.SaveChangesAsync();
            var token = TokenHelper.CreateJWT(User, configuration, jwtOptions);

            return new TokenDTO
            {
                AccessToken = token,
                JwtExpiresIn = DateTime.UtcNow.Add(jwtOptions.JwtLifeTime),
                TokenType= "Bearer",
                RefreshToken = refreshToken,
                RefreshTokenExpiresIn = refreshTokenExpiresIn
            };
        }
        public async Task<TokenDTO> LoginAsync(LoginRequest loginRequest)
        {
            var User = await unitOfWork.Users.GetByUserNameOrEmailAsync(loginRequest.UserNameOrEmail);
            if (User == null || !User.isEmailVerified || !PasswordHasher.Verify(loginRequest.PasswordHash, User.PasswordHash)) 
                    throw new UnauthorizedAccessException("Invalid user name or password.");
            var token = TokenHelper.CreateJWT(User, configuration, jwtOptions);

            return new TokenDTO
            {
                AccessToken = token,
                JwtExpiresIn = DateTime.UtcNow.Add(jwtOptions.JwtLifeTime),
                RefreshToken = User.RefreshToken??"",
                RefreshTokenExpiresIn= User.RefreshTokenExpiryTime?? DateTime.UtcNow
            };

        }

        public async Task<TokenDTO> RefreshTokenAsync(RefreshTokenRequest refreshTokenDRequest)
        {
            var Principal= GetPrincipalFromExpiredToken(refreshTokenDRequest.OldToken);
            int.TryParse(Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId);
            if(userId ==0) throw new SecurityTokenException("Invalid token");

            var user = await unitOfWork.Users.GetByKeysAsync(userId);
            if (user == null || user.RefreshToken!= refreshTokenDRequest.RefreshToken ||user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null!;
            var newAccessToken = TokenHelper.CreateJWT(user, configuration, jwtOptions);
            var newRefreshToken = GenerateRefreshToken();
            var RefreshTokenExpireDate = DateTime.UtcNow.AddDays(jwtOptions.RefreshTokenLifeTime);
            user.SetRefreshToken(newRefreshToken, RefreshTokenExpireDate);
            await unitOfWork.SaveChangesAsync();
            return new TokenDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                TokenType = "Bearer",
                JwtExpiresIn =DateTime.UtcNow.Add(jwtOptions.JwtLifeTime) ,
                RefreshTokenExpiresIn = RefreshTokenExpireDate
            };
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return $"{Convert.ToBase64String(randomNumber)}_{Guid.NewGuid()}";
            }
        }
        public async Task<bool> ForgetPasswordAsync(EmailVerificationRequest verificationRequest)
        {
            var User = await unitOfWork.Users.GetByUserNameOrEmailAsync(verificationRequest.Email);
            if (await SendTokenToEmailAsync(User))//this method add OTP token if he is valid user
            {
                await unitOfWork.SaveChangesAsync();//to save OTP token in Database
                return true;
            }
            return false;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest resetPassRequest)
        {
            var User = await VerifyTokenAsync(new ConfirmEmailVerificationRequest(resetPassRequest.Email, resetPassRequest.Token));
            if (User == null || !User.isEmailVerified) return false;
            var HashedPassword= PasswordHasher.Hash(resetPassRequest.NewPassword);
            User.ChangePassword(HashedPassword);
            User.ClearVerifyToken();
            await unitOfWork.SaveChangesAsync();
            return true;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken)
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        private async Task<bool> SendTokenToEmailAsync(AppUser? User)
        {
            // Always return true to hide if user exists to avoids enumeration attack
            if (User == null)
            {
                await Task.Delay(500);
                return true;
            }
            var otp = OtpGenerator.GenerateSecureOtp();
            User.SetVerifyToken(otp, DateTime.UtcNow.AddMinutes(10));
            try
            {
                await emailService.SendEmailAsync(new SendEmailRequest(User.Email, EmailTemplates.OtpSubject, EmailTemplates.GetOtpBody(otp)));
            }
            catch (Exception ex)
            {
                // log error
                Console.WriteLine(ex.Message);
            }

            return true;
        }
        private bool VerifyToken(ValidateTokenRequest validateTokenRequest)
        {
            return validateTokenRequest.RequestToken == validateTokenRequest.StoredToken && validateTokenRequest.ExpireDate>= DateTime.UtcNow;
        }

        public async Task<IdentityUser> VerifyTokenAsync(ConfirmEmailVerificationRequest ConfirmationRequest)
        {
            if (string.IsNullOrWhiteSpace(ConfirmationRequest.Email)) return null!;
            var User= await unitOfWork.Users.GetByUserNameOrEmailAsync(ConfirmationRequest.Email);
            if (User == null) return null!;
            bool isValidToken= VerifyToken(new ValidateTokenRequest(ConfirmationRequest.Token, User.VerifyToken!, User.VerifyTokenExpires));
            if (!isValidToken) return null!;
            return User;
        }
    }
}
