using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyFavoriteThings.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        public string CategoryGroup { get; set; }
        public int GroupOrder { get; set; }
        public int ItemOrder { get; set; }
        public string CategoryName { get; set; }
    }
}