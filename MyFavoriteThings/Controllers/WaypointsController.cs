using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyFavoriteThings.Models;
using Newtonsoft.Json;

namespace MyFavoriteThings.Controllers
{
    public class WaypointsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool UserIsCreator(int AdventureID)
        {
            // Get the creator of this adventure; if it's the same as the logged-in user, return true
            int adventureCreator = db.Adventures.Where(a => a.AdventureID == AdventureID).First().ContributorID;
            int loggedInContributorID = GetUsersContributorID();
            return adventureCreator == loggedInContributorID;
            //return AdventureID == GetUsersContributorID();
        }
        public int GetUsersContributorID()
        {
            string appUserID = User.Identity.GetUserId();
            if (appUserID == null) return 0;
            return db.Contributors.Where(c => c.ApplicationUserId == appUserID).Select(f => f.ContributorID).First();
        }
        public string[] GetSunriseSunset(string dateString, double latitude, double longitude)
        {


            return new string[] { "6:45am", "7:05pm" };
        }
        // GET: Waypoints
        public ActionResult Index(int id)   //adventureID
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
            ViewBag.ContributorID = GetUsersContributorID();
            ViewBag.UserIsCreator = UserIsCreator(id);

            //string[] sunriseSunset = GetSunriseSunset(DateTime.Today.ToShortDateString, latitude, longitude);

            ViewBag.Sunrise = "6:45am";
            ViewBag.Sunset = "7:05pm";

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
            ViewBag.UserIsCreator = UserIsCreator(waypoint.AdventureID);
            ViewBag.AdventureID = waypoint.AdventureID;
            return View(waypoint);
        }

        // GET: Waypoints/Create
        public ActionResult Create(int id)
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
            ViewBag.AdventureID = id;      // new SelectList(db.Adventures, "AdventureID", "AdventureName");

            Waypoint waypoint = new Waypoint();
            int nextWaypointSequence = db.Waypoints.Where(w => w.AdventureID == id).Count() + 1;
            waypoint.Sequence = nextWaypointSequence;

            return View(waypoint);
        }

        // POST: Waypoints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "WaypointID,WaypointName,WaypointName_Obscure,WaypointNickname,WaypointNickname_Obscure,Description,Description_Obscure,DirectionsNarrative,DirectionsNarrative_Obscure,Lat,Long,Street1,Street2,City,State,Phone,DayTimeOfDayNarrative,AdventureID")] Waypoint waypoint)
        public async Task<ActionResult> Create(Waypoint waypoint)
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
            if (ModelState.IsValid)
            {
                // If lat & long are 0, and there is a street, city, & state, use the GeoDecoderRing to 
                // populate lat & long
                if (waypoint.Lat == 0  && waypoint.Long == 0)
                {
                    if (waypoint.Street1 != null && waypoint.City != null && waypoint.Street1 != null)
                    {
                        if (waypoint.Street1.Trim() != "" && waypoint.City.Trim() != "" && waypoint.Street1.Trim() != "")
                        {
                            double[] coordinates = await GetLatLongArray(waypoint.Street1, waypoint.City, waypoint.State);
                            waypoint.Lat = coordinates[0];
                            waypoint.Long = coordinates[1];
                        }
                    }
                }

                db.Waypoints.Add(waypoint);
                db.SaveChanges();
                // Give them the option of adding another
                ViewBag.AdventureID = waypoint.AdventureID;      // new SelectList(db.Adventures, "AdventureID", "AdventureName");
                //return View();
                return RedirectToAction("Index", new { id = waypoint.AdventureID });
                //return RedirectToAction("Index");
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
            ViewBag.AdventureID = new SelectList(db.Adventures, "AdventureID", "AdventureName", waypoint.AdventureID);
            return View(waypoint);
        }

        // GET: Waypoints/Edit/5
        public ActionResult Edit(int? id)
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
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
            ViewBag.UserIsCreator = UserIsCreator(waypoint.AdventureID);
            ViewBag.AdventureID = waypoint.AdventureID;
            return View(waypoint);
        }

        // POST: Waypoints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "WaypointID,WaypointName,WaypointName_Obscure,WaypointNickname,WaypointNickname_Obscure,Description,Description_Obscure,DirectionsNarrative,DirectionsNarrative_Obscure,Lat,Long,Street1,Street2,City,State,Phone,DayTimeOfDayNarrative,AdventureID")] Waypoint waypoint)
        public async Task<ActionResult> Edit(Waypoint waypoint)
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
            if (ModelState.IsValid)
            {
                // If lat & long are 0, and there is a street, city, & state, use the GeoDecoderRing to 
                // populate lat & long
                if (waypoint.Lat == 0 && waypoint.Long == 0)
                {
                    if (waypoint.Street1 != null && waypoint.City != null && waypoint.Street1 != null)
                    {
                        if (waypoint.Street1.Trim() != "" && waypoint.City.Trim() != "" && waypoint.Street1.Trim() != "")
                        {
                            double[] coordinates = await GetLatLongArray(waypoint.Street1, waypoint.City, waypoint.State);
                            waypoint.Lat = coordinates[0];
                            waypoint.Long = coordinates[1];
                        }
                    }
                }
                db.Entry(waypoint).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = waypoint.AdventureID });
                //return RedirectToAction("Index");
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
            ViewBag.AdventureID = new SelectList(db.Adventures, "AdventureID", "AdventureName", waypoint.AdventureID);
            return View(waypoint);
        }

        // GET: Waypoints/Delete/5
        public ActionResult Delete(int? id)
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
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
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
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

        public async Task<double[]> GetLatLongArray(string streetAddress, string City, string State)
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
            double[] latLng = new double[2];

            // This is the geoDecoderRing 
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(streetAddress.Replace(" ", "+"));
                stringBuilder.Append(";");
                stringBuilder.Append(City.Replace(" ", "+"));
                stringBuilder.Append(";");
                stringBuilder.Append(State.Replace(" ", "+"));
                // example: string url = @"https://maps.googleapis.com/maps/api/geocode/json?address={stringBuilder.ToString()}1600+Amphitheatre+Parkway,+Mountain+View,+CA&key=YOUR_API_KEY";
                string url = @"https://maps.googleapis.com/maps/api/geocode/json?address=" +
                    stringBuilder.ToString() + "&key=" + APIKeys.GeoLocatorAPIKey;

                WebRequest request = WebRequest.Create(url);
                WebResponse response = await request.GetResponseAsync();
                System.IO.Stream data = response.GetResponseStream();
                StreamReader reader = new StreamReader(data);
                // json-formatted string from maps api
                string responseFromServer = reader.ReadToEnd();
                response.Close();

                var root = JsonConvert.DeserializeObject<MapAPIData>(responseFromServer);
                var location = root.results[0].geometry.location;
                //var latitude = location.lat;
                //var longitude = location.lng;
                ////foreach (var singleResult in root.results)
                ////{
                ////    var location = singleResult.geometry.location;
                ////    var latitude = location.lat;
                ////    var longitude = location.lng;
                ////}
                //latLng = { location.lat, location.lng};
                latLng[0] =  location.lat;
                latLng[1] =  location.lng;
                return latLng;
                }
            catch
            {

                latLng[0] = 0;
                latLng[1] = 0;
                return latLng;
            }
        }
    }
    public class MapAPIData
    {
        public Result[] results { get; set; }
        public string status { get; set; }
    }

    public class Result
    {
        public Address_Components[] address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
        public string[] types { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }
    }

    public class Location
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Viewport
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }
    }

    public class Northeast
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Southwest
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Address_Components
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public string[] types { get; set; }
    }
}
