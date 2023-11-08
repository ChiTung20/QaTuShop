using CosmeticsShop.Extensions;
using CosmeticsShop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CosmeticsShop.Controllers
{
    public class HomeController : Controller
    {
        ShoppingEntities db = new ShoppingEntities();
        public ActionResult Index()
        {
            if (Session["User"] != null && (Session["User"] as Models.User).UserTypeID == 1)
            {
                return RedirectToAction("Index", "Admin");
            }
            if (Session["Cart"] == null)
            {
                Session["Cart"] = new List<ItemCart>();
            }
            ViewBag.ListProduct = db.Products
                .Where(x => x.IsActive == true && x.PurchasedCount > 0)
                .Take(8)
                .OrderByDescending(x => x.PurchasedCount)
                .ToList();
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
        //public ActionResult SignUp()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult SignUp(Models.User user)
        //{
        //    Models.User check = db.Users.SingleOrDefault(x => x.Email == user.Email);
        //    if (check != null)
        //    {
        //        ViewBag.Message = "Email đã tồn tại";
        //        return View();
        //    }
           
        //    Models.User userAdded = new Models.User();
        //    try
        //    {
        //        user.Password = HashMD5.ToMD5(user.Password);
        //        user.UserTypeID = 2;
        //        //user.Address = "TPHCM";
        //        user.Avatar = "avatar.jpg";
        //        userAdded = db.Users.Add(user);
        //        db.SaveChanges();
        //    }
        //    catch (Exception)
        //    {
        //        ViewBag.Message = "Đăng ký thất bại";
        //        return View();
        //    }
        //    ViewBag.Message = "Đăng ký thành công";
        //    return View();
        //    //return RedirectToAction("ConfirmEmail", "User", new { ID = userAdded.ID });
        //}

        public ActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignIn(string Email, string Password)
        {
            string stringPassword = HashMD5.ToMD5(Password);
            Models.User check = db.Users.SingleOrDefault(x => x.Email == Email && x.Password == stringPassword);
            if (check != null)
            {
                Session["User"] = check;
                if (check.UserTypeID == 1)
                {
                    return RedirectToAction("Index", "Admin");
                }
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Message = "Email hoặc mật khẩu không đúng";
            return View();
        }
        public ActionResult SignOut()
        {
            Session.Remove("User");
            return RedirectToAction("Index");
        }
        public ActionResult Quiz()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            return View();
        }
        public ActionResult Suggest()
        {
            ViewBag.ListCategory = db.Categories.Where(x => x.IsActive == true).ToList();
            ViewBag.ListProduct = Session["Suggest"] as List<Product>;
            return View();
        }
        [HttpPost]
        public ActionResult Suggest(List<string> data)
        {
            var product = new List<Product>();
            ViewBag.ListCategory = db.Categories.Where(x => x.IsActive == true).ToList();
            var s = string.Join(" ", data);
            if (s.Contains("căng") && s.Contains("đỏ") && s.Contains("ngứa"))
            {
                product = db.Products.Where(x => x.Type == "Hỗn hợp").ToList();
                Session["Suggest"] = product;
                return Json(new { message = "Bạn có làn da hỗn hợp." }, JsonRequestBehavior.AllowGet);
            }
            else if (s.Contains("căng"))
            {
                product = db.Products.Where(x => x.Type == "Dầu").ToList();
                Session["Suggest"] = product;
                return Json(new { message = "Làn da của bạn là da dầu." }, JsonRequestBehavior.AllowGet);
            }
            else if (s.Contains("ngứa"))
            {
                product = db.Products.Where(x => x.Type == "Khô").ToList();
                Session["Suggest"] = product;
                return Json(new { message = "Bạn có một làn da khô" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                product = db.Products.Where(x => x.Type == "Nhạy cảm").ToList();
                Session["Suggest"] = product;
                return Json(new { message = "Bạn có làn da nhạy cảm." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult GetOTP()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetOTP(string recipientEmail)//Quên Mật Khẩu
        {
            if (string.IsNullOrEmpty(recipientEmail))
            {
                ModelState.AddModelError("", "Hãy nhập Email hợp lệ.");
                return View();
            }

            // Kiểm tra tính hợp lệ của địa chỉ email (định dạng Gmail)
            if (!ForgotPassword.IsValidGmailAddress(recipientEmail))
            {
                ModelState.AddModelError("", "Hãy nhập Email hợp lại.");
                return View();
            }

            // Tạo mã OTP ngẫu nhiên
            string otp = ForgotPassword.GenerateOTP();

            // Lưu mã OTP vào cơ sở dữ liệu
            ForgotPassword.SaveOTPToDatabase(recipientEmail, otp);

            // Gửi mã OTP đến người dùng qua email
            string subject = "Xác minh OTP";
            string body = $"Mã OTP của bạn là: {otp}";

            ForgotPassword.SendEmail(recipientEmail, subject, body);

            ViewBag.OTP = recipientEmail;

            // Chuyển hướng đến trang xác thực OTP
            return View();
        }
        // xác minh OTP
        public ActionResult ExampleUsage(string otp)
        {
            var user = db.Users.FirstOrDefault(u => u.OTPRecords == otp);// Lấy người dùng đầu tiên trong danh sách, bạn có thể điều chỉnh truy vấn tùy theo cấu trúc cơ sở dữ liệu của bạn
            if (user != null)
            {
                string userEmail = user.Email; // Địa chỉ email người dùng từ cơ sở dữ liệu

                if (ForgotPassword.VerifyOTP(userEmail, otp))
                {
                    //lấy TempData để tí nữa xử lý phần ResetPassword
                    TempData["OTP"] = otp;
                    ViewBag.Message = "Thành công";
                    return RedirectToAction("ResetPassword");
                }
                else
                {
                    ViewBag.Message = "OTP Sai.";
                    return View();
                }
            }
            else
            {
                ViewBag.Message = "Người dùng không tồn tại trong cơ sở dữ liệu.";
                return View();
            }
        }
        public ActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(string email, string newPassword, string confirmPassword)
        {
            string otp = TempData["OTP"] as string;

            if (newPassword != confirmPassword)
            {
                ViewBag.ErrorMessage = "Mật khẩu không trùng khớp";
                ViewBag.ShowResetForm = true;
                ViewBag.Email = email;
                return View("ForgotPassword");
            }

            var user = db.Users.FirstOrDefault(u => u.OTPRecords == otp);
            if (user != null)
            {
                // cập nhật mật khẩu mới
                user.Password = HashMD5.ToMD5(newPassword);
                db.SaveChanges();

                User check = db.Users.SingleOrDefault(x => x.OTPRecords == otp);
                if (check != null)
                {
                    Session["User"] = check;
                    if (check.UserTypeID == 1)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    return RedirectToAction("Index", "Home");
                }
                // Chuyển hướng đến trang thành công
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ShowResetForm = false;
                return View("ForgotPassword");
            }
        }


        //đăng ký với mã otp
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(string recipientEmail, Models.User user)
        {
            Models.User check = db.Users.SingleOrDefault(x => x.Email == user.Email);
            if (check != null)
            {
                ViewBag.Message = "Email đã tồn tại";
                return View();
            }
            if (!ForgotPassword.IsValidGmailAddress(recipientEmail))//Kiểm tra mail có đúng định dạng
            {
                ModelState.AddModelError("", "hãy nhập Email hợp lệ.");
                ViewBag.Invalid = "lỗi";
                return View();
            }
            LoginWithGmail.SaveEmailToDatabase(recipientEmail);
            // Tạo mã OTP ngẫu nhiên
            string otp = ForgotPassword.GenerateOTP();

            // Lưu mã OTP vào cơ sở dữ liệu
            ForgotPassword.SaveOTPToDatabase(recipientEmail, otp);

            // Gửi mã OTP đến người dùng qua email
            string subject = "Xác minh OTP";
            string body = $"Mã OTP của bạn là: {otp}";

            ForgotPassword.SendEmail(recipientEmail, subject, body);

            ViewBag.OTP = recipientEmail;

            // Chuyển hướng đến trang xác thực OTP
            return View();

        }


       
        [HttpPost]
        public ActionResult UpdatePassword(string otp)
        {
            var user = db.Users.FirstOrDefault(u => u.OTPRecords == otp);
            if (ModelState.IsValid)
            {
                string userEmail = user.Email;
                // Kiểm tra xác thực OTP và lưu mật khẩu mới vào cơ sở dữ liệu
                if (ForgotPassword.VerifyOTP(userEmail, otp))
                {
                    if (user != null)
                    {
                        TempData["OTP"] = otp;
                        ViewBag.Message = "Thành công";

                        return RedirectToAction("PasswordUpdated");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "OTP không hợp lệ. Vui lòng thử lại.");
                }
            }

            return View();
        }
        public ActionResult PasswordUpdated()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PasswordUpdated(string newPassword)
        {
            string otp = TempData["OTP"] as string;

            var user = db.Users.FirstOrDefault(u => u.OTPRecords == otp);
            if (user != null)
            {
                // cập nhật mật khẩu
                user.Name = user.Email;
                user.UserTypeID = 2;
                user.Avatar = "avatar.jpg";
                user.Password = HashMD5.ToMD5(newPassword);               
                db.SaveChanges();

                User check = db.Users.SingleOrDefault(x => x.OTPRecords == otp);
                if (check != null)
                {
                    Session["User"] = check;
                    if (check.UserTypeID == 1)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    return RedirectToAction("Index", "Home");
                }
                //    Models.User userAdded = new Models.User();
                //    try
                //    {
                //        user.Password = HashMD5.ToMD5(user.Password);
                //        user.UserTypeID = 2;
                //        //user.Address = "TPHCM";
                //        user.Avatar = "avatar.jpg";
                //        userAdded = db.Users.Add(user);
                //        db.SaveChanges();
                //    }
                //    catch (Exception)
                //    {
                //        ViewBag.Message = "Đăng ký thất bại";
                //        return View();
                //    }
                //    ViewBag.Message = "Đăng ký thành công";
                //    return View();

                // Chuyển hướng đến trang thành công
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ShowResetForm = false;
                return View("ForgotPassword");
            }
        }

        //public ActionResult SignUp(Models.User user)
        //{
        //    Models.User check = db.Users.SingleOrDefault(x => x.Email == user.Email);
        //    if (check != null)
        //    {
        //        ViewBag.Message = "Email đã tồn tại";
        //        return View();
        //    }

        //    Models.User userAdded = new Models.User();
        //    try
        //    {
        //        user.Password = HashMD5.ToMD5(user.Password);
        //        user.UserTypeID = 2;
        //        //user.Address = "TPHCM";
        //        user.Avatar = "avatar.jpg";
        //        userAdded = db.Users.Add(user);
        //        db.SaveChanges();
        //    }
        //    catch (Exception)
        //    {
        //        ViewBag.Message = "Đăng ký thất bại";
        //        return View();
        //    }
        //    ViewBag.Message = "Đăng ký thành công";
        //    return View();
        //    //return RedirectToAction("ConfirmEmail", "User", new { ID = userAdded.ID });
        //}
    }
}
