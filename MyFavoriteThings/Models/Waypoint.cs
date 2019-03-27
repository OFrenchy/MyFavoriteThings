using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyFavoriteThings.Models
{
    public class Waypoint
    {
        [Key] 
        public int WaypointID { get; set; }
        public int Sequence { get; set; } = 1;
        [Required]
        public string WaypointName { get; set; }
        [Required]
        public string WaypointName_Obscure { get; set; }
        public string WaypointNickname { get; set; }
        public string WaypointNickname_Obscure { get; set; }
        [Required]
        [MaxLength(512)]
        public string Description { get; set; }
        [Required]
        [MaxLength(512)]
        public string Description_Obscure { get; set; }
        [Required]
        [MaxLength(1024)]
        public string DirectionsNarrative { get; set; }
        [Required]
        [MaxLength(1024)]
        public string DirectionsNarrative_Obscure { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }
        //public string URL { get; set; }
        public string DayTimeOfDayNarrative { get; set; }
        //public string TimeOfDayToCalculate { get; set; } //(BSAT, BSNT, BSCT, SR, NOON, SS, ASCT, ASNT, ASAT)

        [ForeignKey("Adventure")]
        public int AdventureID { get; set; }
        public Adventure Adventure { get; set; }

    }
}