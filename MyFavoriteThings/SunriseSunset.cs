using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyFavoriteThings
{
    public class SunriseSunset
    {
        public Result Results { get; set; }
        public string Status { get; set; }
    }
    //  "{\"results\":{\"sunrise\":\"1:55:43 PM\",\"sunset\":\"2:32:16 AM\",\"solar_noon\":\"8:13:59 PM\",\"day_length\":\"12:36:33\",
    //  \"civil_twilight_begin\":\"1:29:21 PM\",\"civil_twilight_end\":\"2:58:38 AM\",\"nautical_twilight_begin\":\"12:58:10 PM\",
    //  \"nautical_twilight_end\":\"3:29:49 AM\",\"astronomical_twilight_begin\":\"12:26:11 PM\",\"astronomical_twilight_end\":\"4:01:47 AM\"},
    //  \"status\":\"OK\"}"

    public class Result
    {
        public string sunrise { get; set; }
        public string sunset { get; set; }

        public string solar_noon { get; set; }
        public string day_length { get; set; }
        public string civil_twilight_begin { get; set; }
        public string civil_twilight_end { get; set; }
        public string nautical_twilight_begin { get; set; }
        public string nautical_twilight_end { get; set; }
        public string astronomical_twilight_begin { get; set; }
        public string astronomical_twilight_end { get; set; }
    }


}