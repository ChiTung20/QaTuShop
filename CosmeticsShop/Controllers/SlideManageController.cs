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

namespace CosmeticsShop.Controllers
{
    public class SlideManageController : Controller
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
            List<Slide> slides = new List<Slide>();
            if (keyword != "")
            {
                slides = db.Slides.Where(x => x.Name.Contains(keyword)).OrderBy(x => x.ID).ToList();
            }
            else
            {
                slides = db.Slides.Where(x => x.Name.Contains(keyword)).OrderBy(x => x.ID).ToList();
            }
            return View(slides);

           
        }
        public ActionResult ToggleActive(int ID)
        {
            Slide slide = db.Slides.Find(ID);
            slide.Status = !slide.Status;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "ID", "Name");
            return View();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Slide slide, HttpPostedFileBase[] ImageUpload)
        {
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
                slide.Image = ImageUpload[0].FileName;
            }
            slide.CreatedBy = (Session["User"] as Models.User).ID;
            slide.Status = true;
            slide.CreatedDate = DateTime.Now;
            db.Slides.Add(slide);
            db.SaveChanges();

            ViewBag.Message = "Thêm thành công";
            return View("Details", slide);
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

            Slide slide = db.Slides.Find(ID);
            if (slide == null)
            {
                return HttpNotFound();
            }
            return View(slide);
        }
        public ActionResult Edit()
        {
            return RedirectToAction("Index");
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Slide slide, HttpPostedFileBase[] ImageUpload)
        {
            Slide slideUpdate = db.Slides.Find(slide.ID);
            slideUpdate.Name = slide.Name;
            slideUpdate.DisplayOrder = slide.DisplayOrder;
            slideUpdate.Description = slide.Description;

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
                slideUpdate.Image = ImageUpload[0].FileName;
            }

            db.SaveChanges();

            ViewBag.Message = "Cập nhật thành công";
            return View("Details", slideUpdate);
        }
        public ActionResult Delete()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Delete(int? ID)
        {
            Slide slide = db.Slides.Find(ID);
            db.Slides.Remove(slide);
            db.SaveChanges();
            ViewBag.Message = "Xoá thành công";
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
