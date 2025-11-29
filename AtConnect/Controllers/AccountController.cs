using AtConnect.BLL.DTOs;
using AtConnect.BLL.Interfaces;
using AtConnect.Core.Models;
using AtConnect.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService authenticationService;

        public AccountController(IAuthenticationService authentication)
        {
            this.authenticationService = authentication;
        }
        [HttpPost("/Login")]
        public async Task<ActionResult<AuthResponse>> LogIn(LoginRequest LoginData)
        {
            return await authenticationService.LoginAsync(LoginData.UserNameOrEmail, LoginData.PasswordHash);
        }
        [HttpPost("/Register")]
        public async Task<ActionResult<AuthResponse>> Register(RegistrationRequest registrationRequest)
        {
            return await authenticationService.RegisterAsync(
                new AppUser(registrationRequest.FirstName, registrationRequest.LastName,
                    registrationRequest.UserName, registrationRequest.Email,
                    registrationRequest.Password));
        }

        [HttpPost("/ForgotPassword")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            if (await authenticationService.ForgetPasswordAsync(forgotPasswordDTO))
                return Ok("The password reset code has been sent to your mail.");
            return BadRequest("Invalid Mail");
        }
        [HttpPost("/VerifyResetToken")]
        public async Task<ActionResult> VerifyResetToken(VerifyResetTokenDTO verifyResetTokenDTO)
        {
            if (await authenticationService.VerifyResetPasswordTokenAsync(verifyResetTokenDTO))
                return Ok("OTP verified successfully. You can now reset your password.");
            return BadRequest("Invalid or expired OTP.");
        }
        [HttpPost("/ResetPassword")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            if (await authenticationService.ResetPasswordAsync(resetPasswordDTO))
                return Ok("Password has been reset successfully.");
            return BadRequest("Invalid or expired OTP.");
        }

        [HttpPost("/RefreshToken")]
        public async Task<ActionResult<AuthResponse>> RefreshToken(RefreshTokenDTO request)
        {
            var authResponse = await authenticationService.RefreshTokenAsync(request);
            if (authResponse == null)
                return Unauthorized("Invalid refresh token.");
            return authResponse;
        }

    }
}
