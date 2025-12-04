using AtConnect.BLL.DTOs;
using AtConnect.BLL.Interfaces;
using AtConnect.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
       
        [HttpPost("Login")]
        public async Task<ActionResult<ResultDTO<AuthResponse>>> LogIn(LoginRequest loginData)
        {
            var result = await _authenticationService.LoginAsync(loginData);
            if (!result.Success)
                return Unauthorized(result);
            return result;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<ResultDTO<RegistrationRequest>>> Register(RegistrationRequest registrationRequest)
        {
            var result = await _authenticationService.RegisterAsync(registrationRequest);
            if(!result.Success)
                return BadRequest(result);

            return result;
        }
  
        [HttpPost("VerifyEmailCode")]
        public async Task<ActionResult<ResultDTO<AuthResponse>>> VerifyEmailToken(ConfirmEmailVerificationRequest verificationToken)
        {
            var result = await _authenticationService.VerifyEmailToken(verificationToken);
            if(!result.Success)
                return BadRequest(result);
            return result;
        }


        [HttpPost("ForgotPassword")]
        public async Task<ActionResult<ResultDTO<Object>>> ForgotPassword(EmailVerificationRequest verifyToken)
        {
            if (await _authenticationService.ForgetPasswordAsync(verifyToken))
                return new ResultDTO<Object>(true,"The password reset code has been sent to your mail.");
            return BadRequest(new ResultDTO<Object>(false, "Invalid Mail"));
        }

        [HttpPost("VerifyResetCode")]
        public async Task<ActionResult<ResultDTO<Object>>> VerifyResetToken(ConfirmEmailVerificationRequest verifyResetTokenDTO)
        {
            if (await _authenticationService.VerifyTokenAsync(verifyResetTokenDTO)== null)
                return BadRequest(new ResultDTO<Object>(false,"Invalid or expired OTP."));
            return new ResultDTO<Object>(true, "OTP verified successfully. You can now reset your password.");
        }
        [HttpPost("ResetPassword")]
        public async Task<ActionResult<ResultDTO<Object>>> ResetPassword(ResetPasswordRequest resetPasswordDTO)
        {
            if (await _authenticationService.ResetPasswordAsync(resetPasswordDTO))
                return new ResultDTO<Object>(true, "Password has been reset successfully.");
            return BadRequest(new ResultDTO<Object>(false, "Invalid or expired OTP."));
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<ResultDTO<AuthResponse>>> RefreshToken(RefreshTokenRequest request)
        {
            var result = await _authenticationService.RefreshTokenAsync(request);
            if (!result.Success)
                return Unauthorized(result);
            return result;
        }
      
    }
}
