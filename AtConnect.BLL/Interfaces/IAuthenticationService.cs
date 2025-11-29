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
        Task<bool> RegisterAsync(AppUser user);
        Task<TokenDTO> LoginAsync(LoginRequest loginRequest);
        Task<TokenDTO> RefreshTokenAsync(RefreshTokenRequest refreshTokenDRequest);
        Task<bool> ForgetPasswordAsync(EmailVerificationRequest forgotPassRequest);
        public  Task<TokenDTO> VerifyEmailToken(ConfirmEmailVerificationRequest verificationRequest);
        Task<IdentityUser> VerifyTokenAsync(ConfirmEmailVerificationRequest ConfirmationRequest);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest resetPassRequest);
    }
}
