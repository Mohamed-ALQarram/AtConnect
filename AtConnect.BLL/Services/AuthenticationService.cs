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

        public async Task<ResultDTO<RegistrationRequest>> RegisterAsync(RegistrationRequest registration)
        {
            string email = registration.Email.Trim().ToLowerInvariant();
            var existingUser = await unitOfWork.Users.GetByUserNameOrEmailAsync(email);
            if (existingUser != null)
            {
                if(existingUser.isEmailVerified)
                {
                    return new(false, "This email already exists.", registration);
                }

                await SendTokenToEmailAsync(existingUser); // this will send the Code to user email and will update our object with the code to save it on Db
                existingUser.ChangeFirstName(registration.FirstName);
                existingUser.ChangeLastName(registration.LastName);
                existingUser.ChangeUserName(registration.UserName);
                existingUser.ChangePassword(PasswordHasher.Hash(registration.Password));
                unitOfWork.Users.Update(existingUser);
                await unitOfWork.SaveChangesAsync();
                return new (true, "The Verification code has been sent to your mail.", registration);
            }

            if (await unitOfWork.Users.CheckUserNameAsync(registration.UserName)) return new (false,"This UserName already exists.", registration);
            var newUser = new AppUser(registration.FirstName, registration.LastName, registration.UserName, registration.Email, PasswordHasher.Hash(registration.Password));


            await SendTokenToEmailAsync(newUser);
            await unitOfWork.Users.AddAsync(newUser);
            await unitOfWork.SaveChangesAsync();

            return new (true, "The Verification code has been sent to your mail.", registration);
        }

        public async Task<ResultDTO<AuthResponse>> VerifyEmailToken(ConfirmEmailVerificationRequest verificationToken)
        {
            var User=  await VerifyTokenAsync(verificationToken);
            if (User == null) return new (false, "Invalid token", null);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiresIn = DateTime.UtcNow.AddDays(jwtOptions.RefreshTokenLifeTime);
            User.EmailVerification(true);
            User.ClearVerifyToken();
            User.SetRefreshToken(refreshToken, refreshTokenExpiresIn);
            await unitOfWork.SaveChangesAsync();
            var token = TokenHelper.CreateJWT(User, configuration, jwtOptions);

            var authResponse= new AuthResponse
            {
                AccessToken = token,
                AccessTokenExpiresAt  = DateTime.UtcNow.Add(jwtOptions.JwtLifeTime),
                TokenType= "Bearer",
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt  = refreshTokenExpiresIn
            };
            return new ResultDTO<AuthResponse>(true, "Your account verified successfully.", authResponse);
        }
        public async Task<ResultDTO<AuthResponse>> LoginAsync(LoginRequest loginRequest)
        {
            var User = await unitOfWork.Users.GetByUserNameOrEmailAsync(loginRequest.UserNameOrEmail.Trim().ToLowerInvariant());
            if (User == null || !User.isEmailVerified || !PasswordHasher.Verify(loginRequest.Password, User.PasswordHash))
                return new(false, "Invalid UserName or password.");
            var token = TokenHelper.CreateJWT(User, configuration, jwtOptions);

            var authResponse= new AuthResponse
            {
                AccessToken = token,
                AccessTokenExpiresAt  = DateTime.UtcNow.Add(jwtOptions.JwtLifeTime),
                RefreshToken = User.RefreshToken??"",
                RefreshTokenExpiresAt = User.RefreshTokenExpiryTime?? DateTime.UtcNow
            };
            return new ResultDTO<AuthResponse>(true,"Logged in Successfully." ,authResponse );    
        }

        public async Task<ResultDTO<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest refreshTokenDRequest)
        {
            var Principal = GetPrincipalFromExpiredToken(refreshTokenDRequest.OldToken);
            if (Principal == null)
                return new(false, "Invalid access token.", null);
            int.TryParse(Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId);

            if (userId ==0) return new(false, "Invalid access token.",null);
            var user = await unitOfWork.Users.GetByKeysAsync(userId);
            if (user == null || user.RefreshToken!= refreshTokenDRequest.RefreshToken ||user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return new(false, "Invalid refresh token.", null);
            var newAccessToken = TokenHelper.CreateJWT(user, configuration, jwtOptions);
            var newRefreshToken = GenerateRefreshToken();
            var RefreshTokenExpireDate = DateTime.UtcNow.AddDays(jwtOptions.RefreshTokenLifeTime);
            user.SetRefreshToken(newRefreshToken, RefreshTokenExpireDate);
            await unitOfWork.SaveChangesAsync();
            var authResponse = new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                TokenType = "Bearer",
                AccessTokenExpiresAt  =DateTime.UtcNow.Add(jwtOptions.JwtLifeTime) ,
                RefreshTokenExpiresAt  = RefreshTokenExpireDate
            };
            return new(true, "Token has been refreshed successfully.", authResponse);
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
                return null!;

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
            
                await emailService.SendEmailAsync(new SendEmailRequest(User.Email, EmailTemplates.OtpSubject, EmailTemplates.GetOtpBody(otp)));

            return true;
        }
        private bool VerifyToken(ValidateTokenRequest validateTokenRequest)
        {
            return validateTokenRequest.RequestToken == validateTokenRequest.StoredToken && validateTokenRequest.ExpireDate>= DateTime.UtcNow;
        }

        public async Task<IdentityUser> VerifyTokenAsync(ConfirmEmailVerificationRequest ConfirmationRequest)
        {
            if (string.IsNullOrWhiteSpace(ConfirmationRequest.Email)) return null!;
            var User= await unitOfWork.Users.GetByUserNameOrEmailAsync(ConfirmationRequest.Email.Trim().ToLowerInvariant());
            if (User == null) return null!;
            bool isValidToken= VerifyToken(new ValidateTokenRequest(ConfirmationRequest.Token, User.VerifyToken!, User.VerifyTokenExpires));
            if (!isValidToken) return null!;
            return User;
        }
    }
}
