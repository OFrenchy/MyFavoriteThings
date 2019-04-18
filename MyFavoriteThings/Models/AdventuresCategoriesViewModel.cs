using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MyFavoriteThings.Models
{
    public class AdventuresCategoriesForIndex
    {
        //AdventuresCategoriesViewModel
        public List<Adventure> Adventures { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
}