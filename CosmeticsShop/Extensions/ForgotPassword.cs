using CosmeticsShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace CosmeticsShop.Extensions
{
    class ForgotPassword
    {
        // Hàm để lấy mã OTP ngẫu nhiên
        public static string GenerateOTP()
        {
            int otpLength = 6; // Độ dài mã OTP
            Random random = new Random();
            string otp = string.Empty;

            for (int i = 0; i < otpLength; i++)
            {
                otp += random.Next(0, 9).ToString();
            }

            return otp;
        }

        // Hàm để lấy mã OTP từ cơ sở dữ liệu
        public static string GetStoredOTP(string userEmail)
        {
            using (var dbContext = new ShoppingEntities()) // Thay thế YourDbContext() bằng lớp DbContext của bạn
            {
                var user = dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
                if (user != null)
                {
                    return user.OTPRecords;
                }
                return null;
            }
        }

        // Hàm để lưu mã OTP vào cơ sở dữ liệu
        public static void SaveOTPToDatabase(string userEmail, string otp)
        {
            using (var dbContext = new ShoppingEntities()) // Thay thế YourDbContext() bằng lớp DbContext của bạn
            {
                var user = dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
                if (user != null)
                {
                    user.OTPRecords = otp;
                    dbContext.SaveChanges();
                }
            }
        }

        //Xác minh OTP
        public static bool VerifyOTP(string userEmail, string otp)
        {
            string storedOTP = GetStoredOTP(userEmail);
            return otp == storedOTP;
        }

        //lấy mã OTP từ View
        public string GetUserEnteredOTP()
        {
            Console.Write("Nhập mã OTP: ");
            string otp = Console.ReadLine();

            return otp;
        }
        //Gmail hợp lệ
        public static bool IsValidGmailAddress(string email)
        {
            // Kiểm tra địa chỉ email có đúng định dạng Gmail hay không
            var gmailPattern = @"^[a-z0-9](\.?[a-z0-9]){5,}@g(oogle)?mail\.com$";
            return Regex.IsMatch(email, gmailPattern, RegexOptions.IgnoreCase);
        }
        //Gửi Email
        public static void SendEmail(string recipientEmail, string subject, string body)
        {
            string senderEmail = "nchitung20sn@gmail.com"; // Địa chỉ email người gửi
            string senderPassword = "wupwvfwpkcypzpsu"; // Mật khẩu email người gửi

            MailMessage mail = new MailMessage();
            mail.To.Add(recipientEmail);
            mail.From = new MailAddress(senderEmail);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
            smtp.EnableSsl = true;

            try
            {
                smtp.Send(mail);
                // Gửi email thành công
            }
            catch (SmtpException ex)
            {
                // Xử lý lỗi khi gửi email
                Console.WriteLine("Lỗi gửi email: " + ex.Message);
            }
        }
    }
}