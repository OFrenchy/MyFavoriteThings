using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyFavoriteThings.Models
{
    public class Adventure
    {
        //
        [Key]
        public int AdventureID { get; set; }
        public string AdventureName { get; set; }
        public string AdventureName_Obscure { get; set; }
        public string AdventureDescription { get; set; }
        public string AdventureDescription_Obscure { get; set; }
        public string AdventureGeneralLocation { get; set; }
        public string AdventureGeneralLocation_Obscure { get; set; }
        public double Rating  { get; set; }
        public int RatingCounter { get; set; }
        public int RatingSum { get; set; }
        public bool AllowComments { get; set; }
        public bool AllowImages { get; set; }
        public string Comments { get; set; }

        [ForeignKey("Contributor")]
        public int ContributorID { get; set; }
        public Contributor Contributor { get; set; }

    }
}