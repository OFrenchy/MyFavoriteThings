using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyFavoriteThings.Models
{
    public class AdventureCategory
    {
        [Key]
        [ForeignKey("Adventure")]
        [Column(Order = 1)]
        public int AdventureID { get; set; }
        public Adventure Adventure { get; set; }

        [Key]
        [ForeignKey("Category")]
        [Column(Order = 2)]
        public int CategoryID { get; set; }
        public Category Category { get; set; }

    }
}