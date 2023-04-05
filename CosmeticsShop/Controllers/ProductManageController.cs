using CosmeticsShop.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using PagedList;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using X.PagedList.Mvc;

namespace CosmeticsShop.Controllers
{
    public class ProductManageController : Controller
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
        public ActionResult Index(int CategoryID = 0,  string keyword = "", int? page = 1)
        {
            if (CheckRole("Admin"))
            {

            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }

            List<Product> products = new List<Product>();
            if (keyword != "")
            {
                products = db.Products.Where(x => x.Name.Contains(keyword)).OrderBy(x => x.ID).ToList();
            }
            if (CategoryID != 0)
            {
                ViewBag.NamePage = "Category " + db.Categories.Find(CategoryID).Name;
                ViewBag.ListProduct = db.Products.Where(x => x.IsActive == true && x.CategoryID == CategoryID).ToList();
            }
            else
            {
                products = db.Products.Where(x => x.Name.Contains(keyword)).OrderBy(x => x.ID).ToList();
            }
            //Phân trang
            if (page == null) page = 1;
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            //int pageNumber = page;
            PagedList<Product> models = new PagedList<Product>(products.AsQueryable(), pageNumber, pageSize);

            //Trang hiện tại
            ViewBag.CurrentPage = pageNumber;
            return View(models);

        }
        public ActionResult ToggleActive(int ID)
        {
            Product product = db.Products.Find(ID);
            product.IsActive = !product.IsActive;
            db.SaveChanges();
            return RedirectToAction("Index");
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
            Product product = db.Products.Find(ID);
            ViewBag.CategoryList = db.Categories.ToList();
            return View(product);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Product product, HttpPostedFileBase[] ImageUpload)
        {
            Product productUpdate = db.Products.Find(product.ID);
            productUpdate.Name = product.Name;
            productUpdate.Price = product.Price;
            productUpdate.Quantity = product.Quantity;
            productUpdate.Type = product.Type;
            productUpdate.CategoryID = product.CategoryID;
            productUpdate.Description = product.Description;

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
                productUpdate.Image1 = ImageUpload[0].FileName;
            }
            if (ImageUpload[1] != null)
            {
                productUpdate.Image2 = ImageUpload[1].FileName;
            }
            if (ImageUpload[2] != null)
            {
                productUpdate.Image3 = ImageUpload[2].FileName;
            }
            db.SaveChanges();

            ViewBag.CategoryList = db.Categories.ToList();
            ViewBag.Message = "Cập nhật thành công";
            return View("Details", productUpdate);
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
            ViewBag.CategoryList = db.Categories.Where(x => x.IsActive == true).ToList();
            return View();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(Product product, HttpPostedFileBase[] ImageUpload)
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
                product.Image1 = ImageUpload[0].FileName;
            }
            if (ImageUpload[1] != null)
            {
                product.Image2 = ImageUpload[1].FileName;
            }
            if (ImageUpload[2] != null)
            {
                product.Image3 = ImageUpload[2].FileName;
            }
            product.CreatedBy = (Session["User"] as Models.User).ID;
            product.ViewCount = 0;
            product.PurchasedCount = 0;
            product.IsActive = true;
            db.Products.Add(product);
            db.SaveChanges();

            ViewBag.CategoryList = db.Categories.Where(x => x.IsActive == true).ToList();
            ViewBag.Message = "Thêm thành công";
            return View("Details", product);
        }
       
        public ActionResult Delete()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Delete(int? ID)
        {
            Product product = db.Products.Find(ID);
            db.Products.Remove(product);
            db.SaveChanges();
            ViewBag.Message = "Xoá thành công";
            return RedirectToAction("Index");
        }
    }
}