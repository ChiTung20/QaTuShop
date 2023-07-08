using CosmeticsShop.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CosmeticsShop.Controllers
{
    public class OrderManageController : Controller
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
        // GET: OrderManage
        public ActionResult Index()
        {
            if (CheckRole("Admin"))
            {

            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
            List<Order> orders = db.Orders.ToList();
            return View(orders);
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
            ViewBag.IsProcessed = (db.Orders.Find(ID).Status == "Processing") ? false : true;
            List<OrderDetail> orderDetails = db.OrderDetails.Where(x => x.OrderID.Value == ID).ToList();
            return View(orderDetails);
        }
        public ActionResult Processed(int ID)
        {
            Order order = db.Orders.Find(ID);
            order.Status = "Processed";
            order.DateShip = DateTime.Now.AddDays(3);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delivering(int ID)
        {
            Order order = db.Orders.Find(ID);
            order.Status = "Delivering";
            order.DateShip = DateTime.Now.AddDays(3);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Delete(int? ID)
        {
            Order order = db.Orders.Find(ID);
            db.Orders.Remove(order);
            db.SaveChanges();
            ViewBag.Message = "Huỷ thành công";
            return RedirectToAction("Index");
        }
    }
}