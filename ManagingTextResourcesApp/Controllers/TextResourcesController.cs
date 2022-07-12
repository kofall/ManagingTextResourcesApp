using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ManagingTextResourcesApp.Models;

namespace ManagingTextResourcesApp.Controllers
{
    public class TextResourcesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Index(int? uId)
        {
            if( uId == null)
            {
                ViewBag.uId = null;
                return RedirectToAction("Login", "Home");
            }
            ViewBag.errorMsg = null;
            ViewBag.users = db.Users.ToList();
            ViewModel vm = new ViewModel();
            vm.TextResources = db.TextResources.ToList();
            if (uId == 0)
            {
                User user = new User();
                user.UserId = 0;
                user.Name = "Gość";
                vm.User = user;
            }
            else
            {
                vm.User = db.Users.Find(uId);
                ActionResult result = checkLogged(vm.User.UserId);
                if (result != null)
                    return result;
            }
            return View(vm);
        }

        public ActionResult Details_EditableLink(string hash)
        {
            TextResource textResource = null;
            if (hash == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            foreach (var res in db.TextResources.ToList())
            {
                if (String.Compare(res.EditHashCode, hash) == 0)
                {
                    textResource = res;
                    break;
                }
            }
            if (textResource == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Details", new { uId = -1, id = textResource.Id, generateLink = false });
        }

        public ActionResult Details(int uId, int? id, bool? generateLink)
        {
            ViewBag.errorMsg = null;
            ActionResult result = checkLogged(uId);
            if (result != null)
                return result;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextResource textResource = db.TextResources.Find(id);
            if (textResource == null)
            {
                return HttpNotFound();
            }
            ViewBag.uId = uId;
            if (generateLink.HasValue && uId > 0)
            {
                if (generateLink.Value)
                {
                    Random random = new Random();
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    bool goodHash = true;
                    do
                    {
                        goodHash = true;
                        textResource.EditHashCode = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
                        foreach(var res in db.TextResources.ToList())
                        {
                            if (res.Id != textResource.Id)
                                if (String.Compare(res.EditHashCode, textResource.EditHashCode) == 0)
                                    goodHash = false;
                        }
                    } while (!goodHash);
                    db.Entry(textResource).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return View(textResource);
        }

        // GET: TextResources/Create
        public ActionResult Create(int uId)
        {
            ViewBag.errorMsg = null;
            ViewBag.uId = uId;
            ActionResult result = checkLogged(uId);
            if (result != null)
                return result;
            return View();
        }

        // POST: TextResources/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int uId, [Bind(Include = "Id,UserId,Title,Text")] TextResource textResource)
        {
            ViewBag.errorMsg = null;
            ViewBag.uId = uId;
            ActionResult result = checkLogged(uId);
            if (result != null)
                return result;
            if (ModelState.IsValid)
            {
                foreach (var text in db.TextResources.ToList())
                {
                    if (text.UserId == uId)
                    {
                        if (String.Compare(text.Title, textResource.Title) == 0)
                        {
                            ViewBag.errorMsg = "Zasób o takim tytule już istnieje.";
                            return View(textResource);
                        }
                        if (String.Compare(text.Text, textResource.Text) == 0)
                        {
                            ViewBag.errorMsg = "Zasób o takiej treści już istnieje.";
                            return View(textResource);
                        }
                    }
                }
                textResource.UserId = uId;
                db.TextResources.Add(textResource);
                db.SaveChanges();
                return RedirectToAction("Index", new { uid = uId });
            }

            return View(textResource);
        }

        public ActionResult Edit(int uId, int tUId, int? id)
        {
            ViewBag.errorMsg = null;
            ViewBag.uId = uId;
            ActionResult result = checkLogged(uId);
            if (result != null)
                return result;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextResource textResource = db.TextResources.Find(id);
            if (textResource == null)
            {
                return HttpNotFound();
            }
            return View(textResource);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int uId, int tUId, string hash, [Bind(Include = "Id,UserId,Title,Text")] TextResource textResource)
        {
            ViewBag.errorMsg = null;
            ViewBag.uId = uId;
            ActionResult result = checkLogged(uId);
            if (result != null)
                return result;
            if (ModelState.IsValid)
            {
                textResource.UserId = tUId;
                textResource.EditHashCode = hash;
                db.Entry(textResource).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    foreach (var res in db.TextResources.ToList())
                    {
                        if (res.UserId == tUId && res.Id != textResource.Id)
                        {
                            if (String.Compare(res.Title, textResource.Title) == 0)
                            {
                                ViewBag.errorMsg = "Zasób o takim tytule już istnieje.";
                                return View(textResource);
                            }
                            if (String.Compare(res.Text, textResource.Text) == 0)
                            {
                                ViewBag.errorMsg = "Zasób o takiej treści już istnieje.";
                                return View(textResource);
                            }
                        }
                    }
                }
                return RedirectToAction("Details", new { uId = uId, id = textResource.Id });
            }
            return View(textResource);
        }

        [ActionName("Delete")]
        public ActionResult Delete(int uId, int id)
        {
            ViewBag.errorMsg = null;
            ViewBag.uId = uId;
            TextResource textResource = db.TextResources.Find(id);
            db.TextResources.Remove(textResource);
            db.SaveChanges();
            return RedirectToAction("Index", new { uId = uId });
        }

        protected override void Dispose(bool disposing)
        {
            ViewBag.errorMsg = null;
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult checkLogged(int uId)
        {
            if (uId > 0)
            {
                if (String.Compare(db.Users.Find(uId).status, "Zalogowany") != 0)
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            return null;
        }

        public ActionResult Logout(int uId)
        {
            ViewBag.errorMsg = null;
            ViewBag.uId = null;
            if (uId != 0)
            {
                User user = null;
                foreach (var x in db.Users.ToList())
                {
                    if (x.UserId == uId)
                    {
                        user = x;
                        break;
                    }
                }
                user.status = "Wylogowany";
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Login", "Home");
        }
    }
}
