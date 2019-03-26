using Microsoft.AspNet.Identity;
using MyFavoriteThings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyFavoriteThings.Controllers
{
    public class ContributorController : Controller
    {
        ApplicationDbContext db;
        public ContributorController()
        {
            db = new ApplicationDbContext();
        }
        // GET: Contributor
        public ActionResult Index()
        {
            return View();
        }

        // GET: Contributor/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Contributor/Create
        public ActionResult Create()
        {
            //ViewBag.ID = new SelectList(db.Contributors, "Id", "Name");
            return View();
        }

        // POST: Contributor/Create
        [HttpPost]
        public ActionResult Create(Contributor contributor)             //FormCollection collection)
        {
            //Creating a Visitor
            try
            {
                if (ModelState.IsValid)
                {
                    contributor.ApplicationUserId = User.Identity.GetUserId();
                    db.Contributors.Add(contributor);
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "Adventures");
                //return RedirectToAction("Index", "Home");
                ////return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
            
        }

        // GET: Contributor/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Contributor/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Contributor/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Contributor/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
