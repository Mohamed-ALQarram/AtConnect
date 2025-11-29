using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.Helper
{
    public static class OtpGenerator
    {
        // Generate secure 6-digit numeric OTP using StringBuilder
        public static string GenerateSecureOtp(int length = 6)
        {
            if (length <= 0)
                throw new ArgumentException("OTP length must be greater than 0.");

            var otpBuilder = new StringBuilder(length);

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomNumber = new byte[4]; // 4 bytes = 32-bit integer

                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(randomNumber);
                    int value = BitConverter.ToInt32(randomNumber, 0) & 0x7FFFFFFF; // positive number
                    otpBuilder.Append(value % 10); // append single digit
                }
            }

            return otpBuilder.ToString();
        }
    }
}
