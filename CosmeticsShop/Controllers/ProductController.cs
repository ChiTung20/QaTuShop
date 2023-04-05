using CosmeticsShop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer.Symbols;
using System.IO;

namespace CosmeticsShop.Controllers
{
    public class ProductController : Controller
    {
        ShoppingEntities db = new ShoppingEntities();
        // GET: Product
        public ActionResult Index(int CategoryID = 0, string keyword = "", int SortPrice = 0, int? page = 1)
        {
            List<Product> lsproducts = new List<Product>();
            //Phân trang
            if (page == null) page = 1;
            int pageSize = 8;
            int pageNumber = (page ?? 1);

            ViewBag.ListCategory = db.Categories.Where(x => x.IsActive == true ).ToList();
            if (keyword != "")
            {
                ViewBag.NamePage = "Search product";
                //ViewBag.ListProduct = db.Products.Where(x => x.IsActive == true && x.Name.Contains(keyword)).ToList();
                //return View();
                lsproducts = db.Products.Where(x => x.IsActive == true && x.Name.Contains(keyword)).ToList();

            }
            else if (CategoryID != 0)
            {
                ViewBag.NamePage = "Category " + db.Categories.Find(CategoryID).Name;
                //ViewBag.ListProduct = db.Products.Where(x => x.IsActive == true && x.CategoryID == CategoryID).ToList();
                //return View();
                lsproducts = db.Products.AsNoTracking().Where(x => x.IsActive == true && x.CategoryID == CategoryID).ToList();
            }
            else if (SortPrice == 1)
            {
                ViewBag.NamePage = "Lower to Higher Price";
                //ViewBag.ListProduct = db.Products.Where(x => x.IsActive == true).OrderBy(x => x.Price).ToList();
                //return View();
                lsproducts = db.Products.Where(x => x.IsActive == true).OrderBy(x => x.Price).ToList();
            }
            else if (SortPrice == 2)
            {
                ViewBag.NamePage = "Higher to Lower Price";
                //ViewBag.ListProduct = db.Products.Where(x => x.IsActive == true).OrderByDescending(x => x.Price).ToList();
                lsproducts = db.Products.Where(x => x.IsActive == true).OrderByDescending(x => x.Price).ToList();
            }
            else
            {
                ViewBag.NamePage = "All products";
                //ViewBag.ListProduct = db.Products.Where(x => x.IsActive == true).ToList();
                //products = db.Products.Where(x => x.IsActive == true).ToList();
                lsproducts = db.Products.Where(x => x.IsActive == true).OrderByDescending(x => x.ID).ToList();
            }

            //int pageNumber = page;
            PagedList<Product> models = new PagedList<Product>(lsproducts.AsQueryable(), pageNumber, pageSize);

            //Trang hiện tại
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        public ActionResult Details(int ID)
        {
            Product product = db.Products.Find(ID);
            return View(product);
        }
        public ActionResult RemoveProduct(int ID)
        {
            Product product = db.Products.Find(ID);
            db.Products.Remove(product);
            return View(product);
        }
    }
}