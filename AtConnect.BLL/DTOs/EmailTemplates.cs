namespace AtConnect.BLL.DTOs
{
        public static class EmailTemplates
        {
            // Subject constants
            public const  string OtpSubject = "Your One-Time Password (OTP)";

            // HTML body template with placeholder for OTP
            public static string GetOtpBody(string OTP)
            {
                return 
                $@"<!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""UTF-8"">
                    <title>Your OTP Code</title>
                    <style>
                        .container {{
                            font-family: Arial, sans-serif;
                            max-width: 500px;
                            margin: auto;
                            padding: 20px;
                            border: 1px solid #ddd;
                            border-radius: 8px;
                        }}
                        .otp {{
                            font-size: 24px;
                            font-weight: bold;
                            color: #2E86C1;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 12px;
                            color: #555;
                        }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <p>Hello,</p>
                        <p>Your OTP code is:</p>
                        <p class=""otp"">{OTP}</p>
                        <p>This OTP is valid for the next 10 minutes. Please do not share it with anyone.</p>
                        <div class=""footer"">
                            If you did not request this code, please ignore this email.<br>
                            Thank you,<br>
                            AtConnect
                        </div>
                    </div>
                </body>
                </html>";
            }
            
        
    }
}
    
    
