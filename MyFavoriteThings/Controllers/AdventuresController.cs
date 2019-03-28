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
            // Adventure1!@abc.com  Adventure2!@abc.com
            ViewBag.ContributorID = GetUsersContributorID();
            var adventures = db.Adventures.Include(a => a.Contributor);
            return View(adventures.ToList());
        }

        // GET: Adventures/Details/5
        public ActionResult Details(int? id)
        {
            // Adventure1!@abc.com  Adventure2!@abc.com
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adventure adventure = db.Adventures.Find(id);
            if (adventure == null)
            {
                return HttpNotFound();
            }
            // if the adventure wasn't created by this contributor, disable the Update & Edit links
            ViewBag.UserIsCreator = UserIsCreator(id ?? 0);  //was id ?? 0
            return View(adventure);
        }

        // TODO - find a scenario where these next two methods can be shared amongst controllers
        //      - there's a problem moving it to a static class because of the required db & User objects
        public bool UserIsCreator(int AdventureID)
        {
            // Adventure1!@abc.com  Adventure2!@abc.com
            //int contributorID = GetUsersContributorID();
            //return AdventureID == GetUsersContributorID();
            int adventureCreator = db.Adventures.Where(a => a.AdventureID == AdventureID).First().ContributorID;
            int loggedInContributorID = GetUsersContributorID();
            return adventureCreator == loggedInContributorID;
        }
        public int GetUsersContributorID()
        {
            string appUserID = User.Identity.GetUserId();
            if (appUserID == null) return 0;
            return db.Contributors.Where(c => c.ApplicationUserId == appUserID).Select(f => f.ContributorID).First();
        }
        // GET: Adventures/Create
        public ActionResult Create()
        {
            // Adventure1!@abc.com  Adventure2!@abc.com
            // Pass the ContributorID to the view
            ViewBag.ContributorID = GetUsersContributorID();
            Adventure adventure = new Adventure();
            adventure.Rating = 0;
            adventure.RatingCounter = 0;
            adventure.RatingSum = 0;
            return View(adventure);
        }

        // POST: Adventures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "AdventureID,AdventureName,AdventureName_Obscure,AdventureDescription,AdventureDescription_Obscure,AdventureGeneralLocation,AdventureGeneralLocation_Obscure,Rating,RatingCounter,RatingSum,AllowComments,AllowImages,Comments,ContributorID")] Adventure adventure)
        public ActionResult Create(Adventure adventure)
        {
            if (ModelState.IsValid)
            {
                // Adventure1!@abc.com  Adventure2!@abc.com
                db.Adventures.Add(adventure);
                db.SaveChanges();
                return RedirectToAction("Create", "Waypoints", new { id = adventure.AdventureID });
            }
            else
            {
                foreach (var obj in ModelState.Values)
                {
                    foreach (var error in obj.Errors)
                    {
                        if (!string.IsNullOrEmpty(error.ErrorMessage))
                            System.Diagnostics.Debug.WriteLine("ERROR WHY = " + error.ErrorMessage);
                    }
                }
            }
            return RedirectToAction("Index");
            //ViewBag.ContributorID = new SelectList(db.Contributors, "ContributorID", "FirstName", adventure.ContributorID);
            //return View(adventure);
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
            ViewBag.ContributorID = adventure.ContributorID;   // new SelectList(db.Contributors, "ContributorID", "FirstName", adventure.ContributorID);
            return View(adventure);
        }

        // POST: Adventures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "AdventureID,AdventureName,AdventureName_Obscure,AdventureDescription,AdventureDescription_Obscure,AdventureGeneralLocation,AdventureGeneralLocation_Obscure,Rating,RatingCounter,RatingSum,AllowComments,AllowImages,Comments,ContributorID")] Adventure adventure)
        public ActionResult Edit(Adventure adventure)
        {
            ViewBag.ContributorID = adventure.ContributorID;
            if (ModelState.IsValid)
            {
                db.Entry(adventure).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = adventure.AdventureID });
            }
            //ViewBag.ContributorID = adventure.ContributorID; // new SelectList(db.Contributors, "ContributorID", "FirstName", adventure.ContributorID);
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
