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
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(Models.User user)
        {
            Models.User check = db.Users.SingleOrDefault(x => x.Email == user.Email);
            if (check != null)
            {
                ViewBag.Message = "Email đã tồn tại";
                return View();
            }
           
            Models.User userAdded = new Models.User();
            try
            {
                user.Password = HashMD5.ToMD5(user.Password);
                user.UserTypeID = 2;
                user.Address = "TPHCM";
                user.Avatar = "avatar.jpg";
                userAdded = db.Users.Add(user);
                db.SaveChanges();
            }
            catch (Exception)
            {
                ViewBag.Message = "Đăng ký thất bại";
                return View();
            }
            ViewBag.Message = "Đăng ký thành công";
            return View();
            //return RedirectToAction("ConfirmEmail", "User", new { ID = userAdded.ID });
        }
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
    }
}