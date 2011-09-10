using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace _4sqtransit.Resources
{
    public class OneTransitAPI
    {
        public static dynamic GetAgencies()
        {
            System.Net.WebClient client = new System.Net.WebClient();
            var jsonResult = client.DownloadString("http://www.onetransitapi.com/v1/agencies/getList");
            var result = Json.Decode(jsonResult);

            return result.GetAgenciesResult;
        }

        public static dynamic GetStopsByLocation(string agencyid, double latitude, double longitude, double radius)
        {
            System.Net.WebClient client = new System.Net.WebClient();
            var jsonResult = client.DownloadString("http://www.onetransitapi.com/v1/stops/getListByLocation?agency=" + agencyid + "&lat=" + latitude + "&lon=" + longitude + "&radius=" + radius);
            var result = Json.Decode(jsonResult);

            return result.GetStopsByLocationResult;
        }

        public static dynamic GetStopTimes(string agencyid, string stopid)
        {
            System.Net.WebClient client = new System.Net.WebClient();
            var jsonResult = client.DownloadString("http://www.onetransitapi.com/v1/stops/getTimes?agency=" + agencyid + "&stop=" + stopid);
            var result = Json.Decode(jsonResult);

            return result.GetStopTimesResult;
        }
    }
}