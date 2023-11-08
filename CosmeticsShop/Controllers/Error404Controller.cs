using CosmeticsShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CosmeticsShop.Controllers
{
    public class Error404Controller : Controller
    {
        // GET: Slide
        ShoppingEntities db = new ShoppingEntities();
        public ActionResult Error()
        {     
            return View();
        }
     
    }
}