using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyFavoriteThings.Models;

namespace MyFavoriteThings.Controllers
{
    public class AdventuresController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Adventures
        public ActionResult Index()
        {
            var adventures = db.Adventures.Include(a => a.Contributor);
            return View(adventures.ToList());
        }

        // GET: Adventures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adventure adventure = db.Adventures.Find(id);
            if (adventure == null)
            {
                return HttpNotFound();
            }
            return View(adventure);
        }

        // GET: Adventures/Create
        public ActionResult Create()
        {
            // Adventure1!@abc.com
            // Pass the ContributorID to the view
            //ViewBag.ContributorID = new SelectList(db.Contributors, "ContributorID", "FirstName");
            string thisUserID = User.Identity.GetUserId();
            var thisContributor = db.Contributors.Where(w => w.ApplicationUserId == thisUserID).First();
            ViewBag.ContributorID = thisContributor.ContributorID;
            return View();
        }

        // POST: Adventures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AdventureID,AdventureName,AdventureName_Obscure,AdventureDescription,AdventureDescription_Obscure,AdventureGeneralLocation,AdventureGeneralLocation_Obscure,Rating,RatingCounter,RatingSum,AllowComments,AllowImages,Comments,ContributorID")] Adventure adventure)
        {
            if (ModelState.IsValid)
            {
                db.Adventures.Add(adventure);
                db.SaveChanges();

                // STOP - TODO - move them to add the waypoints
                
                // TODO - decide whether to pass the AdventureID or the new Waypoint
                Waypoint waypoint = new Waypoint();
                waypoint.AdventureID = adventure.AdventureID;
                return RedirectToAction("Create", "Waypoints", new { AdventureID = adventure.AdventureID });

                //return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
            ViewBag.ContributorID = new SelectList(db.Contributors, "ContributorID", "FirstName", adventure.ContributorID);
            return View(adventure);
        }

        // GET: Adventures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adventure adventure = db.Adventures.Find(id);
            if (adventure == null)
            {
                return HttpNotFound();
            }
            ViewBag.ContributorID = new SelectList(db.Contributors, "ContributorID", "FirstName", adventure.ContributorID);
            return View(adventure);
        }

        // POST: Adventures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AdventureID,AdventureName,AdventureName_Obscure,AdventureDescription,AdventureDescription_Obscure,AdventureGeneralLocation,AdventureGeneralLocation_Obscure,Rating,RatingCounter,RatingSum,AllowComments,AllowImages,Comments,ContributorID")] Adventure adventure)
        {
            if (ModelState.IsValid)
            {
                db.Entry(adventure).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ContributorID = new SelectList(db.Contributors, "ContributorID", "FirstName", adventure.ContributorID);
            return View(adventure);
        }

        // GET: Adventures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adventure adventure = db.Adventures.Find(id);
            if (adventure == null)
            {
                return HttpNotFound();
            }
            return View(adventure);
        }

        // POST: Adventures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Adventure adventure = db.Adventures.Find(id);
            db.Adventures.Remove(adventure);
            db.SaveChanges();
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
