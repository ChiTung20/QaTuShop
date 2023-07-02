using CosmeticsShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace CosmeticsShop.Extensions
{
     class LoginWithGmail
    {
        //kiểm tra email tồn tại chưa
        public static bool IsEmailExists(string email)
        {
            using (var dbContext = new ShoppingEntities()) 
            {
                var user = dbContext.Users.FirstOrDefault(u => u.Email == email);
                return user != null;
            }
        }
        public static void SaveEmailToDatabase(string email)
        {
            using (var dbContext = new ShoppingEntities())
            {
                // User để lưu thông tin người dùng
                var user = new User
                {
                    Email = email
                };

                dbContext.Users.Add(user);
                dbContext.SaveChanges();
            }
        }
    }
}