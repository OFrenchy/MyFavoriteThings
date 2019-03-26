//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Web;

//namespace MyFavoriteThings.Models
//{
//    public class Photo
//    {
//        [Key]
//        public int PhotoID { get; set; }
//        public string FileName { get; set; }
//        //public Photo WhateverDataTypeForImageObject { get; set; }
//        //https://stackoverflow.com/questions/24797485/how-to-download-image-from-url
//        // assume we will copy the file into the App_Data folder


//        [ForeignKey("Contributor")]
//        public int ContributorID { get; set; }
//        public Contributor Contributor { get; set; }

//        [ForeignKey("Adventure")]
//        public int AdventureID { get; set; }
//        public Adventure Adventure { get; set; }
//    }
//}