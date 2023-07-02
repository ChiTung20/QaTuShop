using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer.Symbols;
using CosmeticsShop.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using PagedList;
using CosmeticsShop.Extensions;

namespace CosmeticsShop.Controllers
{
    public class UserManageController : Controller
    {
        private ShoppingEntities db = new ShoppingEntities();
        public bool CheckRole(string type)
        {
            Models.User user = Session["User"] as Models.User;
            if (user != null && user.UserType.Name == type)
            {
                return true;
            }
            return false;
        }
        public ActionResult Index(string keyword = "")
        {
            if (CheckRole("Admin"))
            {

            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
            List<User> users = new List<User>();

            if (keyword != "")
            {
                users = db.Users.Where(x => x.Name.Contains(keyword)&& x.UserType.ID >1).OrderBy(x => x.ID).ToList();
            }
            else
            {
                users = db.Users.Where(x => x.Name.Contains(keyword) && x.UserType.ID > 1).OrderBy(x => x.ID).ToList();
            }
            return View(users);


        }

        public ActionResult Details(int ID)
        {
            if (CheckRole("Admin"))
            {

            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
            User user = db.Users.Find(ID);
            ViewBag.UserTypeList = db.UserTypes.ToList();
            return View(user);
        }
       

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(User user, HttpPostedFileBase[] ImageUpload)
        {
            User userUd = db.Users.Find(user.ID);
            userUd.Name = user.Name;
            userUd.Email = user.Email;
            userUd.Phone = user.Phone;
            userUd.Address = user.Address;
            //userUd.CategoryID = user.CategoryID;
            userUd.UserTypeID = user.UserTypeID;

            for (int i = 0; i < ImageUpload.Length; i++)
            {
                //Check content image
                if (ImageUpload[i] != null && ImageUpload[i].ContentLength > 0)
                {
                    //Get file name
                    var fileName = Path.GetFileName(ImageUpload[i].FileName);
                    //Get path
                    var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                    //Check exitst
                    if (!System.IO.File.Exists(path))
                    {
                        //Add image into folder
                        ImageUpload[i].SaveAs(path);
                    }
                }
            }

            if (ImageUpload[0] != null)
            {
                userUd.Avatar = ImageUpload[0].FileName;
            }
            db.SaveChanges();

            ViewBag.UserTypeList = db.UserTypes.ToList();
            ViewBag.Message = "Cập nhật thành công";
            return View("Details", userUd);
        }
        public ActionResult Edit()
        {
            return RedirectToAction("Index");
        }
        public ActionResult Delete()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Delete(int? ID)
        {
            User users = db.Users.Find(ID);
            db.Users.Remove(users);
            db.SaveChanges();
            ViewBag.Message = "Xoá thành công";
            return RedirectToAction("Index");
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
                //user.Address = "TPHCM";
                user.Avatar = "avatar.jpg";
                userAdded = db.Users.Add(user);
                db.SaveChanges();
            }
            catch (Exception)
            {
                ViewBag.Message = "Đăng ký thất bại";
                return RedirectToAction("Index", "UserManage");
            }
            ViewBag.Message = "Đăng ký thành công";
            return RedirectToAction("Index", "UserManage");
            //return RedirectToAction("ConfirmEmail", "User", new { ID = userAdded.ID });
        }

        public ActionResult SignUpAdmin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUpAdmin(Models.User user)
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
                user.UserTypeID = 1;
                //user.Address = "TPHCM";
                user.Avatar = "avatar.jpg";
                userAdded = db.Users.Add(user);
                db.SaveChanges();
            }
            catch (Exception)
            {
                ViewBag.Message = "Đăng ký thất bại";
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.Message = "Đăng ký thành công";
            return RedirectToAction("Index", "Admin");
            //return RedirectToAction("ConfirmEmail", "User", new { ID = userAdded.ID });
        }
        public ActionResult Infor()
        {
            Models.User user = Session["User"] as Models.User;
            return View(user);
        }
        [HttpPost]
        public ActionResult Infor(User user, HttpPostedFileBase[] ImageUpload)
        {
            var u = db.Users.Find((Session["User"] as Models.User).ID);
            u.Name = user.Name;
            u.Address = user.Address;
            u.Email = user.Email;
            u.Phone = user.Phone;
            for (int i = 0; i < ImageUpload.Length; i++)
            {
                //Check content image
                if (ImageUpload[i] != null && ImageUpload[i].ContentLength > 0)
                {
                    //Get file name
                    var fileName = Path.GetFileName(ImageUpload[i].FileName);
                    //Get path
                    var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                    //Check exitst
                    if (!System.IO.File.Exists(path))
                    {
                        //Add image into folder
                        ImageUpload[i].SaveAs(path);
                    }
                }
            }

            if (ImageUpload[0] != null)
            {
                u.Avatar = ImageUpload[0].FileName;
            }
            db.SaveChanges();
            ViewBag.Message = "Cập nhật thành công!";
            Session["User"] = u;
            return RedirectToAction("Index", "Admin");
        }
    }
}