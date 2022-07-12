using ManagingTextResourcesApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagingTextResourcesApp.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Login()
        {
            ViewBag.errorMsg = null;
            ViewBag.uId = null;
            return View();
        }

        [HttpPost]
        public ActionResult Login([Bind(Include = "UserId,Name,Password,status")] User user)
        {
            ViewBag.errorMsg = null;
            ViewBag.uId = null;

            if (ModelState.IsValid)
            {
                List<User> users = db.Users.ToList();
                foreach (var x in users)
                {
                    if (x.Name == user.Name && x.Password == user.Password)
                    {
                        x.status = "Zalogowany";
                        db.Entry(x).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index", "TextResources", new { uId = x.UserId });
                    }
                    else if (x.Name == user.Name)
                    {
                        ViewBag.errorMsg = "Hasło jest niepoprawne.";
                        return View();
                    }
                }
                ViewBag.errorMsg = "Użytkownik o podanej nazwie nie istnieje.";;
            }
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register([Bind(Include = "UserId,Name,Password,status")] User user)
        {
            ViewBag.errorMsg = null;
            if (ModelState.IsValid)
            {
                List<User> users = db.Users.ToList();
                foreach(var x in users)
                {
                    if (String.Compare(x.Name, user.Name) == 0)
                    {
                        ViewBag.errorMsg = "Użytkownik o podanej nazwie już istnieje.";
                        return View();
                    }
                }
                user.status = "Zalogowany";
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index", "TextResources", new { uId = user.UserId });
            }

            return View();
        }

        public ActionResult Guest()
        {
            return RedirectToAction("Index", "TextResources", new { uId = 0 });
        }
    }
}