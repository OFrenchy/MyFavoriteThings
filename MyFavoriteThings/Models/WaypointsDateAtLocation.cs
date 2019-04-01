using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyFavoriteThings.Models
{
    public class WaypointsDateAtLocation
    {
        public List<Waypoint> Waypoints { get; set; }
        //public Waypoint Waypoints { get; set; }
        public string DateAtLocation { get; set; }
        public int AdventureID { get; set; }
    }
}