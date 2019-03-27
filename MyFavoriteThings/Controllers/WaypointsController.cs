﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyFavoriteThings.Models;

namespace MyFavoriteThings.Controllers
{
    public class WaypointsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Waypoints
        public ActionResult Index(int id)   //adventureID
        {
            // Adventure1!@abc.com

            // STOP - add order by Sequence
            var waypoints = db.Waypoints.Include(w => w.Adventure).Where(w => w.AdventureID == id).OrderBy(w => w.Sequence);
            ViewBag.AdventureID = id;
            return View(waypoints.ToList());
        }

        // GET: Waypoints/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Waypoint waypoint = db.Waypoints.Find(id);
            if (waypoint == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdventureID = waypoint.AdventureID;
            return View(waypoint);
        }

        // GET: Waypoints/Create
        public ActionResult Create(int id)
        {
            // Adventure1!@abc.com
            ViewBag.AdventureID = id;      // new SelectList(db.Adventures, "AdventureID", "AdventureName");
            return View();
        }

        // POST: Waypoints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WaypointID,WaypointName,WaypointName_Obscure,WaypointNickname,WaypointNickname_Obscure,Description,Description_Obscure,DirectionsNarrative,DirectionsNarrative_Obscure,Lat,Long,Street1,Street2,City,State,Phone,DayTimeOfDayNarrative,AdventureID")] Waypoint waypoint)
        {
            // Adventure1!@abc.com
            if (ModelState.IsValid)
            {
                db.Waypoints.Add(waypoint);
                db.SaveChanges();
                // Give them the option of adding another
                ViewBag.AdventureID = waypoint.AdventureID;      // new SelectList(db.Adventures, "AdventureID", "AdventureName");
                //return View();
                return RedirectToAction("Index", new { id = waypoint.AdventureID });
                return RedirectToAction("Index");
            }

            ViewBag.AdventureID = new SelectList(db.Adventures, "AdventureID", "AdventureName", waypoint.AdventureID);
            return View(waypoint);
        }

        // GET: Waypoints/Edit/5
        public ActionResult Edit(int? id)
        {
            // Adventure1!@abc.com
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Waypoint waypoint = db.Waypoints.Find(id);
            if (waypoint == null)
            {
                return HttpNotFound();
            }
            //ViewBag.AdventureID = new SelectList(db.Adventures, "AdventureID", "AdventureName", waypoint.AdventureID);
            ViewBag.AdventureID = waypoint.AdventureID;
            return View(waypoint);
        }

        // POST: Waypoints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WaypointID,WaypointName,WaypointName_Obscure,WaypointNickname,WaypointNickname_Obscure,Description,Description_Obscure,DirectionsNarrative,DirectionsNarrative_Obscure,Lat,Long,Street1,Street2,City,State,Phone,DayTimeOfDayNarrative,AdventureID")] Waypoint waypoint)
        {
            // Adventure1!@abc.com
            if (ModelState.IsValid)
            {
                db.Entry(waypoint).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = waypoint.AdventureID });
                //return RedirectToAction("Index");
            }
            ViewBag.AdventureID = new SelectList(db.Adventures, "AdventureID", "AdventureName", waypoint.AdventureID);
            return View(waypoint);
        }

        // GET: Waypoints/Delete/5
        public ActionResult Delete(int? id)
        {
            // Adventure1!@abc.com
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Waypoint waypoint = db.Waypoints.Find(id);
            if (waypoint == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdventureID = waypoint.AdventureID;
            return View(waypoint);
        }

        // POST: Waypoints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Adventure1!@abc.com
            Waypoint waypoint = db.Waypoints.Find(id);
            db.Waypoints.Remove(waypoint);
            db.SaveChanges();
            //ViewBag.AdventureID = waypoint.AdventureID;
            return RedirectToAction("Index", new { id = waypoint.AdventureID });
            //return RedirectToAction("Index");
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