using Newtonsoft.Json;
using PlacePopularity.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace PlacePopularity.Logic
{
    public static class HttpRequest
    {
        #region consts
        private const string USER_AGENT = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_1) " +
                                  "AppleWebKit/537.36 (KHTML, like Gecko) " +
                                  "Chrome/54.0.2840.98 Safari/537.36";
        #endregion 

        public static RootObject GetPlaceDetails(string placeDetailUrl)
        {
            try
            {
                // get a response from google
                string username = "user";
                string password = "pass";
                string encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));

                WebRequest request = WebRequest.Create(placeDetailUrl);
                request.Headers.Add("Authorization", "Basic " + encoded);
                using (WebResponse response = request.GetResponse())
                {

                    using (Stream data = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(data))
                        {
                            string responseFromServer = sr.ReadToEnd();
                            return JsonConvert.DeserializeObject<RootObject>(responseFromServer);
                        }
                    }
                }
            }
            catch { return null; }
        }

        public static string GetPopularityTimes(string placeUrl)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(placeUrl);
                request.UserAgent = USER_AGENT;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

                using (WebResponse response = request.GetResponse())
                {

                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(stream))
                        {
                            // json-formatted string from maps api
                            return sr.ReadToEnd();
                        }
                    }
                }

            }
            catch { return null; }
        }
    }
}
