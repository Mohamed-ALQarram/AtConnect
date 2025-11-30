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
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService authenticationService;

        public AccountController(IAuthenticationService authentication)
        {
            this.authenticationService = authentication;
        }
        [HttpPost("Login")]
        public async Task<ActionResult<TokenResponse>> LogIn(LoginRequest LoginData)
        {
                var Tokens = await authenticationService.LoginAsync(LoginData);
            return new TokenResponse(true, "Logged in Successfully.", Tokens);
        }
        [HttpPost("Register")]
        public async Task<ActionResult<ResponseDTO>> Register(RegistrationRequest registrationRequest)
        {
            var User = new AppUser(registrationRequest.FirstName, registrationRequest.LastName,
                    registrationRequest.UserName, registrationRequest.Email,
                    registrationRequest.Password);
                
            if (!await authenticationService.RegisterAsync(User))
                return BadRequest(new ResponseDTO(false,"Invalid Mail"));

            return new ResponseDTO(true,"The Verification code has been sent to your mail.");
        }

  
        [HttpPost("Email/VerifyToken")]
        public async Task<ActionResult<TokenResponse>> VerifyEmailToken(ConfirmEmailVerificationRequest verificationToken)
        {
            var AuthResponse = await authenticationService.VerifyEmailToken(verificationToken);
            if(AuthResponse == null)
                    return BadRequest(new TokenResponse(false,"Invalid Mail", null!));
           return Ok(new TokenResponse(true, "Your account verified successfully.", AuthResponse));
        }


        [HttpPost("ForgotPassword")]
        public async Task<ActionResult<ResponseDTO>> ForgotPassword(EmailVerificationRequest verifyToken)
        {
            if (await authenticationService.ForgetPasswordAsync(verifyToken))
                return new ResponseDTO(true,"The password reset code has been sent to your mail.");
            return BadRequest(new ResponseDTO(false, "Invalid Mail"));
        }

        [HttpPost("VerifyResetToken")]
        public async Task<ActionResult<ResponseDTO>> VerifyResetToken(ConfirmEmailVerificationRequest verifyResetTokenDTO)
        {
            if (await authenticationService.VerifyTokenAsync(verifyResetTokenDTO)== null)
                    return BadRequest(new ResponseDTO(false,"Invalid or expired OTP."));
                return new ResponseDTO(true, "OTP verified successfully. You can now reset your password.");
        }
        [HttpPost("ResetPassword")]
        public async Task<ActionResult<ResponseDTO>> ResetPassword(ResetPasswordRequest resetPasswordDTO)
        {
            if (await authenticationService.ResetPasswordAsync(resetPasswordDTO))
                return new ResponseDTO(true, "Password has been reset successfully.");
            return BadRequest(new ResponseDTO(false, "Invalid or expired OTP."));
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<TokenResponse>> RefreshToken(RefreshTokenRequest request)
        {
            var authResponse = await authenticationService.RefreshTokenAsync(request);
            if (authResponse == null)
                return Unauthorized(new TokenResponse(false,"Invalid refresh token.",null!));
            return new TokenResponse(true, "Token has been refreshed successfully.", authResponse);
        }

    }
}
