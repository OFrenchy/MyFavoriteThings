using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyFavoriteThings.Controllers;
using System.Web.Mvc;

namespace MyFavoriteThings.Models
{
    public class AdventureCategoriesViewModel
    {
        public Adventure Adventure { get; set; }
        
        public int[] SelectedCategoriesIds { get; set; }

        public List<SelectListItem> Categories { get; set; }
        
        public bool ShowDetail { get; set; }


    }
}