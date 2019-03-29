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
using System.Text;
using System.Data.SqlClient;

namespace MyFavoriteThings.Controllers
{
    public class AdventuresController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Adventures
        public ActionResult Index()
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
            ViewBag.ContributorID = GetUsersContributorID();
            var adventures = db.Adventures.Include(a => a.Contributor);
            return View(adventures.ToList());
        }

        // GET: Adventures/Details/5
        public ActionResult Details(int? id)
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
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
        public ActionResult Follow(int id)
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
            int followerID = GetUsersContributorID();
            // If there is NOT a record for this pair already in the Follow table/model, add it
            // We will allow a contributor to follow him/herself
            int countOfRecords = db.Follows.Where(f => f.ContributorID == id && f.FollowerContributorID == followerID).Count();
            if (countOfRecords == 0)
            {
                try
                {
                    Follow followRecord = new Follow();// new { id, followerID }
                    followRecord.ContributorID = id;
                    followRecord.FollowerContributorID = followerID;
                    db.Follows.Add(followRecord); 
                    db.SaveChanges();
                }
                catch
                {
                    return RedirectToAction("Index", "Adventures");
                }
            }
            return RedirectToAction("Index", "Adventures");
        }

        // TODO - find a scenario where these next two methods can be shared amongst controllers
        //      - there's a problem moving it to a static class because of the required db & User objects
        public bool UserIsCreator(int AdventureID)
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
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

        public List<SelectListItem> GetAllCategories()
        {
            List<SelectListItem> categoryItems = new List<SelectListItem>();
            foreach (var categoryItem in db.Categories)
            {
                categoryItems.Add(new SelectListItem { Text = categoryItem.CategoryName, Value = categoryItem.CategoryID.ToString() });
            }
            return categoryItems;
        }

        // GET: Adventures/Create
        public ActionResult Create()
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
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
                // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
                db.Adventures.Add(adventure);
                db.SaveChanges();

                int numberOfEmailsSent = NotifyFollowers(adventure.ContributorID);

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
        public int NotifyFollowers(int contributorID)
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
            int countOfEmailsSent = 0;
            // Get the list of follower email addresses
            // SQL statement:
            // SELECT B.FirstName as FollowerFirstName, AspNetUsers.Email, 
            //A.FirstName as ContributorFirstName, 'NameOfNewAdventure' as NewAdventureName
            //FROM Contributors A INNER JOIN
            //Follows on A.ContributorID = Follows.ContributorID INNER JOIN
            //Contributors B on Follows.FollowerContributorID = B.ContributorID INNER JOIN
            //AspNetUsers on B.ApplicationUserId = AspNetUsers.Id
            //WHERE A.ContributorID = 1
            
            StringBuilder getGarametersForEmailSQL = new StringBuilder();
            //followers
            getGarametersForEmailSQL.Clear();
            getGarametersForEmailSQL.Append("SELECT B.FirstName as FollowerFirstName, AspNetUsers.Email, ");
            getGarametersForEmailSQL.Append("A.FirstName as ContributorFirstName ");
            //getGarametersForEmailSQL.Append($"'{Adventure.name}' as NewAdventureName ");
            getGarametersForEmailSQL.Append("FROM Contributors A INNER JOIN ");
            getGarametersForEmailSQL.Append("Follows ON A.ContributorID = Follows.ContributorID INNER JOIN ");
            getGarametersForEmailSQL.Append("Contributors B ON Follows.FollowerContributorID = B.ContributorID INNER JOIN ");
            getGarametersForEmailSQL.Append("AspNetUsers ON B.ApplicationUserId = AspNetUsers.Id ");
            getGarametersForEmailSQL.Append($"WHERE A.ContributorID = {contributorID}");
            var followers = db.Database.SqlQuery<EmailRecord>(getGarametersForEmailSQL.ToString(), contributorID);
            if (followers == null)
            {
                // STOP - TODO - redirect??
                return 0;
            }

            if (followers.Count() > 0)// != null)
            {
                try
                {
                    
                    foreach(var thisEmail in followers)
                    {
                        string thisFollowerFirstName = thisEmail.FollowerFirstName;
                        string thisContributorFirstName = thisEmail.ContributorFirstName;
                        string thisAddress = thisEmail.Email;




                    }
                    
                    //    using (var client = new MailKit.Net.Smtp.SmtpClient())
                    //    {
                    //        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    //        // TODO - determine whose mail account we're going to use, plug in the details below
                    //        client.Connect("smtp.friends.com", 587, false);
                    //        client.Authenticate("joey", "password");
                    //        foreach (var thisRecord in peopleToContact)
                    //        {
                    //            var message = new MimeMessage();
                    //            message.From.Add(new MailboxAddress("", ""));  // TODO - determine whose mail account we're going to use
                    //            message.To.Add(new MailboxAddress("", ""));
                    //            message.Subject = "";  //"Subject" text box 

                    //            message.Body = new TextPart("plain")
                    //            {
                    //                Text = @""  //"MessageBody" text box ; replace "Dear <FirstName>," with "Dear Jack," from the user's FirstName field
                    //            };
                    //            client.Send(message);
                    //        }
                    //        client.Disconnect(true);
                    //    }






                }
                catch
                {
                    return countOfEmailsSent;
                }
            }

            return countOfEmailsSent;
        }
        // GET: Adventures/Edit/5
        public ActionResult Edit(int? id)
        {
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
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
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
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
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
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
            // Adventure1!@abc.com  Adventure2!@abc.com Adventure3!@abc.com
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

    public class EmailRecord
    {
        public string FollowerFirstName { get; set; }
        public string Email { get; set; }
        public string ContributorFirstName { get; set; }
    }

}
