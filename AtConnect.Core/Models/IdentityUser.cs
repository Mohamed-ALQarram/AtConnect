using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.Core.Models
{
    public class IdentityUser
    {
        protected IdentityUser() { } 

        public IdentityUser(string userName, string email, string passwordHash)
        {
            UserName = userName;
            Email = email;
            PasswordHash = passwordHash;
        }

        public int Id { get; private set; }
        public string UserName { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public bool isEmailVerified { get; private set; } = false;
        public string PasswordHash { get; private set; } = null!;
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; } = null;
        public string? VerifyToken { get; private set; } //This Token will be used for both Email Verification, and Reset Password Token
        public DateTime? VerifyTokenExpires { get; private set; } = null;

        public void ChangeEmail(string newEmail)
        {
            Email = newEmail;
        }
        public void EmailVerification(bool isVerified)
        {
            isEmailVerified = isVerified;
        }

        public void ChangeUserName(string newUserName)
        {
            UserName = newUserName;
        }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
        }

        public void SetRefreshToken(string token, DateTime expiry)
        {
            RefreshToken = token;
            RefreshTokenExpiryTime = expiry;
        }

        public void ClearRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiryTime = null;
        }

        public void SetVerifyToken(string token, DateTime expiry)
        {
            VerifyToken = token;
            VerifyTokenExpires = expiry;
        }

        public void ClearVerifyToken()
        {
            VerifyToken = null;
            VerifyTokenExpires = null;
        }

    }
}
