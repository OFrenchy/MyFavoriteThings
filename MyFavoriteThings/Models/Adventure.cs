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
        // ???? TODO - find out if ID or Id is best practice - and why not AdventureID or AdventureId
        [Key]
        public int AdventureID { get; set; }
        [Required]
        public string AdventureName { get; set; }
        [Required]
        public string AdventureName_Obscure { get; set; }
        [Required]
        public string AdventureDescription { get; set; }
        [Required]
        public string AdventureDescription_Obscure { get; set; }
        [Required]
        public string AdventureGeneralLocation { get; set; }
        [Required]
        public string AdventureGeneralLocation_Obscure { get; set; }
        public double Rating { get; set; } = 0;
        public int RatingCounter { get; set; } = 0;
        public int RatingSum { get; set; } = 0;
        public bool AllowComments { get; set; }
        public bool AllowImages { get; set; }
        [MaxLength(1024)]
        public string Comments { get; set; }

        //??? TODO - what do these lines actually mean/do
        [ForeignKey("Contributor")]                     // from Model/Table Contributor?
        public int ContributorID { get; set; }          // get this field name, right?
        public Contributor Contributor { get; set; }    // especially this one I don't understand, unless 

    }
}