using AtConnect.BLL.DTOs;
using AtConnect.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthResponse> RegisterAsync(AppUser user);
        Task<AuthResponse> LoginAsync(string UserNameOrEmail, string password);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenDTO refreshTokenDTO);
        Task<AuthResponse?> GetUserProfileAsync(int userId);
        Task<bool> ForgetPasswordAsync(ForgotPasswordDTO forgotPasswordDTO);
        Task<bool> VerifyResetPasswordTokenAsync(VerifyResetTokenDTO verifyResetTokenDTO);

        public bool VerifyResetPasswordToken(VerifyResetTokenRequest verifyResetTokenRequest);
        Task<bool> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
    }
}
