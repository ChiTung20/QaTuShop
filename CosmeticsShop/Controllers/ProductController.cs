using CosmeticsShop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer.Symbols;
using System.IO;
using Model.ViewModel;

namespace CosmeticsShop.Controllers
{
    public class ProductController : Controller
    {

        ShoppingEntities db = new ShoppingEntities();
 
        // GET: Product
        public ActionResult Index(int CategoryID = 0, string keyword = "", int SortPrice = 0, int? page = 1, string currentSearch = "")
        {
            List<Product> lsproducts = new List<Product>();
            //Phân trang
            //if (page == null) page = 1;
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            if (keyword != null)
            {
                page = 1;
            }
            else
            {
                keyword = currentSearch;
            }
            ViewBag.ListCategory = db.Categories.Where(x => x.IsActive == true).ToList();           
            if (keyword != "")
            //if (!string.IsNullOrEmpty(keyword))
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
            else if (SortPrice == 3)
            {
                ViewBag.NamePage = "Giá Dưới 300 Ngàn";
                //ViewBag.ListProduct = db.Products.Where(x => x.IsActive == true).OrderByDescending(x => x.Price).ToList();
                lsproducts = db.Products.Where(x => x.IsActive == true && x.Price < 300000).OrderBy(x => x.Price).ToList();
            }
            else if (SortPrice == 4)
            {
                ViewBag.NamePage = "Giá Từ 300 Ngàn Đến Dưới 700 Ngàn";
                //ViewBag.ListProduct = db.Products.Where(x => x.IsActive == true).OrderByDescending(x => x.Price).ToList();
                lsproducts = db.Products.Where(x => x.IsActive == true && x.Price >= 300000 && x.Price < 700000).OrderBy(x => x.Price).ToList();
            }
            else if (SortPrice == 5)
            {
                ViewBag.NamePage = "Giá Từ 700 Ngàn Đến Dưới 1 Triệu";
                //ViewBag.ListProduct = db.Products.Where(x => x.IsActive == true).OrderByDescending(x => x.Price).ToList();
                lsproducts = db.Products.Where(x => x.IsActive == true && x.Price >= 700000 && x.Price < 1000000).OrderBy(x => x.Price).ToList();
            }
            else if (SortPrice == 6)
            {
                ViewBag.NamePage = "Giá Trên 1 Triệu";
                //ViewBag.ListProduct = db.Products.Where(x => x.IsActive == true).OrderByDescending(x => x.Price).ToList();
                lsproducts = db.Products.Where(x => x.IsActive == true && x.Price >= 1000000).OrderBy(x => x.Price).ToList();
            }

            else
            {
                ViewBag.NamePage = "All products";
                //ViewBag.ListProduct = db.Products.Where(x => x.IsActive == true).ToList();
                //products = db.Products.Where(x => x.IsActive == true).ToList();
                lsproducts = db.Products.Where(x => x.IsActive == true).OrderByDescending(x => x.ID).ToList();
            }
            ViewBag.currentSearch = keyword;
            ViewBag.currentCate = CategoryID;
            ViewBag.SortPrice = SortPrice;

            //int pageNumber = page;
            PagedList<Product> models = new PagedList<Product>(lsproducts.AsQueryable(), pageNumber, pageSize);

            //Trang hiện tại
            ViewBag.CurrentPage = pageNumber;  
            //return View(lsproducts.ToPagedList(pageNumber, pageSize));
            return View(models);
        }

        public ActionResult Details(int ID/*, int detailid*/)
        {
            Product product = db.Products.Find(ID);          
            return View(product);
        }


        [HttpPost]
        public ActionResult AddComment(Comment comments)
        {
            Comment cmm =db.Comments.Add(comments);
            db.SaveChanges();
            ViewBag.Message = "Thêm thành công";
            return View("Details", cmm);
        }
      
        public ActionResult RemoveProduct(int ID)
        {
            Product product = db.Products.Find(ID);
            db.Products.Remove(product);
            return View(product);
        }


    }
}