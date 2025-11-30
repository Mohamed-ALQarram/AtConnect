using AtConnect.BLL.DTOs;
using AtConnect.Core.Models;
using AtConnect.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ResultDTO<object>> RegisterAsync(RegistrationRequest user);
        Task<ResultDTO<AuthResponse>> LoginAsync(LoginRequest loginRequest);
        Task<ResultDTO<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest refreshTokenDRequest);
        Task<bool> ForgetPasswordAsync(EmailVerificationRequest forgotPassRequest);
        public  Task<ResultDTO<AuthResponse>> VerifyEmailToken(ConfirmEmailVerificationRequest verificationRequest);
        Task<IdentityUser> VerifyTokenAsync(ConfirmEmailVerificationRequest ConfirmationRequest);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest resetPassRequest);
    }
}
