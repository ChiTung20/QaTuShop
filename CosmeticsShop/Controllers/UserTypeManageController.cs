using CosmeticsShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CosmeticsShop.Controllers
{
    public class UserTypeManageController : Controller
    {
        ShoppingEntities db = new ShoppingEntities();
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
            List<UserType> userTypes = new List<UserType>();
            if (keyword != "")
            {
                //userTypes = db.UserType.Where(x => x.Name.Contains(keyword)).ToList();
                userTypes = db.UserTypes.Where(x => x.Name.Contains(keyword) && x.ID > 1).ToList();
            }
            else
            {
                userTypes = db.UserTypes.Where(x => x.Name.Contains(keyword) && x.ID > 1).ToList();
            }
            return View(userTypes);
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
            UserType userType = db.UserTypes.Find(ID);
            return View(userType);
        }
        [HttpPost]
        public ActionResult Edit(UserType userTypes)
        {
            UserType userTypeUd = db.UserTypes.Find(userTypes.ID);
            userTypeUd.Name = userTypes.Name;
            db.SaveChanges();
            ViewBag.Message = "Cập nhật thành công";
            return RedirectToAction("Index");
        }
        public ActionResult Edit()
        {
            return RedirectToAction("Index");
        }
        public ActionResult Add()
        {
            if (CheckRole("Admin"))
            {

            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Add(UserType userTypes)
        {
            userTypes.isActive = true;
           UserType us = db.UserTypes.Add(userTypes);
            db.SaveChanges();
            ViewBag.Message = "Thêm thành công";
            //return View("Details", us);
            return RedirectToAction("Index");
        }

        public ActionResult Delete()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Delete(int? ID)
        {
            UserType userType = db.UserTypes.Find(ID);
            db.UserTypes.Remove(userType);
            db.SaveChanges();
            ViewBag.Message = "Xoá thành công";
            return RedirectToAction("Index");
        }

    }
    }