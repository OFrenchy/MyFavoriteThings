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
using System.Net.Http;

namespace MyFavoriteThings.Controllers
{
    public class WaypointsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static readonly HttpClient client = new HttpClient();

        public bool UserIsCreator(int AdventureID)
        {
            // Get the creator of this adventure; if it's the same as the logged-in user, return true
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
        
        public async Task<ActionResult> CalculateSunriseSunset(int aID, string dateString)
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
            string[] sunriseSunset = await GetSunriseSunsetForDateAtWaypoint(aID, dateString);// "4/1/2019");
            ViewBag.Sunrise = sunriseSunset[0]; // "6:45am";
            ViewBag.Sunset = sunriseSunset[1];  // "7:05pm";
            ViewBag.DateForSunriseSunset = DateTime.Parse(dateString).ToShortDateString();  //"4/1/2019";// 
            return View();
        }
        public async Task<string[]> GetSunriseSunsetForDateAtWaypoint(int aID, string dateString)
        {
            var firstWaypoint = db.Waypoints.Where(w => w.AdventureID == aID).FirstOrDefault();
            string[] sunriseSunset = await GetSunriseSunset(DateTime.Parse(dateString).ToShortDateString(), firstWaypoint.Lat, firstWaypoint.Long);
            return sunriseSunset; //  GetSunriseSunset(DateTime.Today.ToShortDateString(), firstWaypoint.Lat, firstWaypoint.Long);
        }

        public async Task<string[]>  GetSunriseSunset(string dateString, double latitude, double longitude)
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
            DateTime dateToSend = DateTime.Parse(dateString);

            //https://api.sunrise-sunset.org/json?lat=36.7201600&lng=-4.4203400&date=2019-03-31
            string urlToSend = $"https://api.sunrise-sunset.org/json?lat={latitude}&lng={longitude}&date={dateToSend.Year.ToString()}-{dateToSend.Month.ToString("00")}-{dateToSend.Day.ToString("00")}";
            var responseString = await client.GetStringAsync(urlToSend);

            var root = JsonConvert.DeserializeObject<SunriseSunset>(responseString);
            string sunrise = root.Results.sunrise;/// .geometry.location;
            string sunset = root.Results.sunset;

            // times are in Zulu = UTC, get the offset for location & DST; Google requires a timestamp
            double timeStamp = ToTimeStamp.ToTimestamp(dateToSend);
            urlToSend = $"https://maps.googleapis.com/maps/api/timezone/json?location={latitude},{longitude}&timestamp={timeStamp.ToString()}&sensor=false&key={APIKeys.GeoLocatorAPIKey}";
            responseString = await client.GetStringAsync(urlToSend);
            var root2 =  JsonConvert.DeserializeObject<GoogleTimeZone>(responseString);
            //{\n   \"dstOffset\" : 3600,\n   \"rawOffset\" : -28800,\n   \"status\" : \"OK\",\n   \"timeZoneId\" : \"America/Los_Angeles\",\n   \"timeZoneName\" : \"Pacific Daylight Time\"\n}\n
            double totalHoursOffset = (root2.rawOffset + root2.dstOffset) / 3600;

            // Add the offset to the time
            sunrise = DateTime.Parse(sunrise).AddHours(totalHoursOffset).ToString("h:mm tt");
            sunset = DateTime.Parse(sunset).AddHours(totalHoursOffset).ToString("h:mm tt") + " (" + root2.timeZoneName +")";
            
            return new string[] { sunrise, sunset };
        }

        // GET: Waypoints
        public async Task<ActionResult> Index(int id, bool showDetail)   //adventureID   , string dateString
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
            ViewBag.ContributorID = GetUsersContributorID();
            ViewBag.UserIsCreator = UserIsCreator(id);
            ViewBag.ShowDetail = showDetail;

            string dateString = DateTime.Today.ToShortDateString();// "4/1/2019"; //dateString ?? DateTime.Today.ToShortDateString();
            
            // get the sunrise/sunset for today from the API
            string[] sunriseSunset = await GetSunriseSunsetForDateAtWaypoint(id, dateString);// DateTime.Parse(dateString).ToShortDateString());
            ViewBag.Sunrise = sunriseSunset[0]; // "6:45am";
            ViewBag.Sunset = sunriseSunset[1];  // "7:05pm";

            var waypoints = db.Waypoints.Include(w => w.Adventure).Where(w => w.AdventureID == id).OrderBy(w => w.Sequence);
            ViewBag.AdventureID = id;
            
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
            
            // create a dictionary with the following items:
            string sqlString = $"SELECT Sequence AS MapPointNumber, WaypointNickname{(showDetail ? "" : "_Obscure")} AS WaypointNickname, Lat, Long FROM Waypoints WHERE AdventureID = {id} ORDER BY Sequence;";
            //var mapPointsData = db.Database.SqlQuery<MapWaypointsData>($"SELECT Sequence AS MapPointNumber, WaypointNickname{(showDetail ? "" : "_Obscure")} AS WaypointNickname, Lat, Long FROM Waypoints WHERE AdventureID = {id} ORDER BY Sequence;").ToArray();
            var mapPointsData = db.Database.SqlQuery<MapWaypointsData>(sqlString).ToArray();

            ViewBag.MapPointsData = mapPointsData;
            //{ coordinate: new mapkit.Coordinate(37.8184493, -122.478409), title: "Golden Gate Bridge", phone: "+1 (415) 921-5858", url: "http://www.goldengatebridge.org" },
            ViewBag.MapKitCode = APIKeys.AppleMapKitToken;

            WaypointsDateAtLocation waypointsDateAtLocation = new WaypointsDateAtLocation();
            waypointsDateAtLocation.Waypoints = waypoints.ToList();
            waypointsDateAtLocation.DateAtLocation = dateString;    // DateTime.Today.ToShortDateString();
            return View(waypointsDateAtLocation);
        }


        [HttpPost]
        public async Task<ActionResult> Index(WaypointsDateAtLocation waypointsDateAtLocation, bool showDetail )
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
            ViewBag.ContributorID = GetUsersContributorID();
            ViewBag.UserIsCreator = UserIsCreator(waypointsDateAtLocation.AdventureID);
            ViewBag.ShowDetail = showDetail;
            string dateString = waypointsDateAtLocation.DateAtLocation;

            // get the sunrise/sunset for today from the API
            string[] sunriseSunset = await GetSunriseSunsetForDateAtWaypoint(waypointsDateAtLocation.AdventureID, dateString);// DateTime.Parse(dateString).ToShortDateString());
            ViewBag.Sunrise = sunriseSunset[0];
            ViewBag.Sunset = sunriseSunset[1];
            var waypoints = db.Waypoints.Include(w => w.Adventure).Where(w => w.AdventureID == waypointsDateAtLocation.AdventureID).OrderBy(w => w.Sequence);
            ViewBag.AdventureID = waypointsDateAtLocation.AdventureID;

            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com

            // create a dictionary with the following items:
            string sqlString = $"SELECT Sequence AS MapPointNumber, WaypointNickname{(showDetail ? "" : "_Obscure")} AS WaypointNickname, Lat, Long FROM Waypoints WHERE AdventureID = {waypointsDateAtLocation.AdventureID} ORDER BY Sequence;";
            //var mapPointsData = db.Database.SqlQuery<MapWaypointsData>($"SELECT Sequence AS MapPointNumber, WaypointNickname{(showDetail ? "" : "_Obscure")} AS WaypointNickname, Lat, Long FROM Waypoints WHERE AdventureID = {id} ORDER BY Sequence;").ToArray();
            var mapPointsData = db.Database.SqlQuery<MapWaypointsData>(sqlString).ToArray();

            ViewBag.MapPointsData = mapPointsData;
            //{ coordinate: new mapkit.Coordinate(37.8184493, -122.478409), title: "Golden Gate Bridge", phone: "+1 (415) 921-5858", url: "http://www.goldengatebridge.org" },
            ViewBag.MapKitCode = APIKeys.AppleMapKitToken;


            waypointsDateAtLocation.Waypoints = waypoints.ToList();
            waypointsDateAtLocation.DateAtLocation = dateString;
            return View(waypointsDateAtLocation);
        }

        // GET: Waypoints/Details/5
        public ActionResult Details(int? id, bool showDetail)
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
            ViewBag.ShowDetail = showDetail;
            return View(waypoint);
        }

        // GET: Waypoints/Create
        public ActionResult Create(int id)
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
            ViewBag.AdventureID = id; 

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
                return RedirectToAction("Index", new { id = waypoint.AdventureID, showDetail = true });
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
        public ActionResult Edit(int? id, bool showDetail )
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
            ViewBag.UserIsCreator = UserIsCreator(waypoint.AdventureID);
            ViewBag.AdventureID = waypoint.AdventureID;
            ViewBag.ShowDetail = showDetail;
            return View(waypoint);
        }

        // POST: Waypoints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "WaypointID,WaypointName,WaypointName_Obscure,WaypointNickname,WaypointNickname_Obscure,Description,Description_Obscure,DirectionsNarrative,DirectionsNarrative_Obscure,Lat,Long,Street1,Street2,City,State,Phone,DayTimeOfDayNarrative,AdventureID")] Waypoint waypoint)
        public async Task<ActionResult> Edit(Waypoint waypoint, bool showDetail)
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
                bool showDetail2 = showDetail;
                db.Entry(waypoint).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.ShowDetail = showDetail;
                return RedirectToAction("Index", new { id = waypoint.AdventureID, showDetail = showDetail2 });
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
            return RedirectToAction("Index", new { id = waypoint.AdventureID, showDetail = false });
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

    public class MapWaypointsData
    {
        public int MapPointNumber { get; set; }
        public string WaypointNickname { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
    }


}
