using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
//using System.Data.Entity.ModelConfiguration.Configuration;

namespace MyFavoriteThings.Models
{
    public class Contributor
    {
        [Key]
        public int ContributorID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
    //public sealed class ManyToManyAssociationMappingConfiguration : System.Data.Entity.ModelConfiguration.Configuration.AssociationMappingConfiguration

}
