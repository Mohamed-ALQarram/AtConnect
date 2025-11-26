using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.DAL.Models
{
    public class IdentityUser
    {
        protected IdentityUser() { } // for EF Core

        public IdentityUser(string userName, string email, string passwordHash)
        {
            UserName = userName;
            Email = email;
            PasswordHash = passwordHash;
        }

        public int Id { get; private set; }
        public string UserName { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;

        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }
        public string? PasswordResetToken { get; private set; }
        public DateTime? ResetTokenExpires { get; private set; }

        // -- Change Email --
        public void ChangeEmail(string newEmail)
        {
            Email = newEmail;
        }

        // -- Change Username --
        public void ChangeUserName(string newUserName)
        {
            UserName = newUserName;
        }

        // -- Change Password (hashed) --
        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
        }

        //        REFRESH TOKEN MANAGEMENT
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

        //      PASSWORD RESET TOKEN MANAGEMENT
        public void SetPasswordResetToken(string token, DateTime expiry)
        {
            PasswordResetToken = token;
            ResetTokenExpires = expiry;
        }

        public void ClearPasswordResetToken()
        {
            PasswordResetToken = null;
            ResetTokenExpires = null;
        }

    }
}
