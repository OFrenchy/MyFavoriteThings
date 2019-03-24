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
        [ForeignKey("Contributor")]
        public int ContributorID { get; set; }
        public Contributor Contributor { get; set; }

        [Key]
        [ForeignKey("Contributor")]
        public int FollowsContributorID { get; set; }
        public Contributor FollowsContributor { get; set; }
    }
}