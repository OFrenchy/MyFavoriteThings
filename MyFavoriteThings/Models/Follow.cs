using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace MyFavoriteThings.Models
{
    public class Follow
    {
        [Key]
        [ForeignKey("Contributor")]                             // ??? TODO - how do I get this to build?  
        [Column(Order = 1)]
        public int ContributorID { get; set; }
        public Contributor Contributor { get; set; }

        [Key]
        //[ForeignKey("Contributor2")]                            
        //[ForeignKey("Contributor as FollowsContributor")]
        [Column(Order = 2)]
        public int FollowerContributorID { get; set; }           //FollowerContributorID
        //public Contributor FollowsContributor { get; set; }
    }
}