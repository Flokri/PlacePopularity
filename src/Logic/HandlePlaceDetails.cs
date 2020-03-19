using Newtonsoft.Json.Linq;
using PlacePopularity.Extensions;
using PlacePopularity.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PlacePopularity.Logic
{
    public static class HandlePlaceDetails
    {
        #region consts
        private const string BASE_URL = "https://maps.googleapis.com/maps/api/place/";
        private const string RADAR_URL = BASE_URL + "radarsearch/json?location={0},{1}&radius={2}&types={3}&key={4}";
        private const string NEARBY_URL = BASE_URL + "nearbysearch/json?location={0},{1}&radius={2}&types={3}&key={4}";
        private const string DETAIL_URL = BASE_URL + "details/json?placeid={0}&key={1}";
        #endregion

        #region publics
        public static PopularityInfo GetPopularity(string apiKey, string placeId)
        {
            return ConstructPlaceUrl(apiKey, placeId);
        }
        #endregion

        #region privates
        private static PopularityInfo ConstructPlaceUrl(string apiKey, string placeId)
        {
            RootObject serverResponse = HttpRequest.GetPlaceDetails(string.Format(DETAIL_URL, placeId, apiKey));

            if (serverResponse == null)
                throw new NullReferenceException("Could not load the details for the specified place.");

            return GetPopularityTimesByDetails(serverResponse.result);
        }

        private static PopularityInfo GetPopularityTimesByDetails(Result details)
        {
            // get the detail address from the specified place, if the formatted address is null or empty use the vicinity
            string address = !string.IsNullOrEmpty(details.formatted_address) ? details.formatted_address : details.vicinity;

            // get the place id
            var placeId = $"{details.name} {address}";

            JObject detailJson = new JObject(
                    new JProperty("id", details.place_id),
                    new JProperty("name", details.name),
                    new JProperty("address", address),
                    new JProperty("types", details.types),
                    new JProperty("coordinates",
                            new JObject(
                                new JProperty("lat", details.geometry.location.lat),
                                new JProperty("lng", details.geometry.location.lng)
                                )
                        )
                );

            return GetPopulartimesFromSearch(placeId, details.name);
        }

        private static PopularityInfo GetPopulartimesFromSearch(string placeId, string name)
        {
            string serverResponse = HttpRequest.GetPopularityTimes(ConstructUrlForPopularitySearch(placeId));

            // decode the response to utf8
            JObject jsonResponse = ServerResponseToJson(serverResponse.ToUtf8());
            if (jsonResponse == null)
                throw new NullReferenceException("The response data could not be converted to a json object.");

            // get the important nodes from the json object
            // if google changes the api this HAS TO BE MODIFIED TO
            var jsonData = jsonResponse["d"];
            var jsonInfo = jsonData[0][1][0][14];

            return new PopularityInfo
            {
                PlaceId = placeId,
                Name = name,
                Rating = GetJsonValue<double>(jsonInfo, new List<int> { 4, 7 }),
                TotalRatings = GetJsonValue<int>(jsonInfo, new List<int> { 4, 8 }),
                Types = GetJsonValue<List<string>>(jsonInfo, new List<int> { 13 }),
                CurrentPopularity = GetJsonValue<int?>(jsonInfo, new List<int> { 84, 7, 1 }) ?? -1,
                CurrentPopularityStatus = GetJsonValue<string>(jsonInfo, new List<int> { 84, 6 }),
                AverageTimeSpent = GetJsonValue<string>(jsonInfo, new List<int> { 117, 0 }),
                UsuallyWaitingTime = GetUsuallyWaitingTime(jsonInfo),
                OpeningHours = GetOpeningHours(jsonInfo)
            };
        }

        private static string ConstructUrlForPopularitySearch(string placeId)
        {
            // request information for a place and parse current popularity
            Dictionary<string, string> urlParams = new Dictionary<string, string>()
            {
                {"tbm","map"},
                {"tch","1"},
                {"hl","en"},
                {"q",Uri.EscapeDataString(placeId).Replace("%20","+")},
                {"pb", "!4m12!1m3!1d4005.9771522653964!2d-122.42072974863942!3d37.8077459796541!2m3!1f0!2f0!3f0!3m2!1i1125!2i976" +
                                        "!4f13.1!7i20!10b1!12m6!2m3!5m1!6e2!20e3!10b1!16b1!19m3!2m2!1i392!2i106!20m61!2m2!1i203!2i100!3m2!2i4!5b1" +
                                        "!6m6!1m2!1i86!2i86!1m2!1i408!2i200!7m46!1m3!1e1!2b0!3e3!1m3!1e2!2b1!3e2!1m3!1e2!2b0!3e3!1m3!1e3!2b0!3e3!" +
                                        "1m3!1e4!2b0!3e3!1m3!1e8!2b0!3e3!1m3!1e3!2b1!3e2!1m3!1e9!2b1!3e2!1m3!1e10!2b0!3e3!1m3!1e10!2b1!3e2!1m3!1e" +
                                        "10!2b0!3e4!2b1!4b1!9b0!22m6!1sa9fVWea_MsX8adX8j8AE%3A1!2zMWk6Mix0OjExODg3LGU6MSxwOmE5ZlZXZWFfTXNYOGFkWDh" +
                                        "qOEFFOjE!7e81!12e3!17sa9fVWea_MsX8adX8j8AE%3A564!18e15!24m15!2b1!5m4!2b1!3b1!5b1!6b1!10m1!8e3!17b1!24b1!" +
                                        "25b1!26b1!30m1!2b1!36b1!26m3!2m2!1i80!2i92!30m28!1m6!1m2!1i0!2i0!2m2!1i458!2i976!1m6!1m2!1i1075!2i0!2m2!" +
                                        "1i1125!2i976!1m6!1m2!1i0!2i0!2m2!1i1125!2i20!1m6!1m2!1i0!2i956!2m2!1i1125!2i976!37m1!1e81!42b1!47m0!49m1" +
                                        "!3b1"}
            };

            return "https://www.google.de/search?" + String.Join("&", urlParams.Select(x => x.Key + "=" + x.Value).ToArray());
        }

        private static JObject ServerResponseToJson(string response)
        {
            try
            {
                var modifiedStr = response.Split(new String[] { @"/*""*/" }, StringSplitOptions.None)[0];
                modifiedStr = modifiedStr.Replace(@"\n", "");
                modifiedStr = modifiedStr.Replace(@"\", string.Empty);
                modifiedStr = modifiedStr.Replace("\")]}'", "");

                int index = modifiedStr.LastIndexOf("]");

                modifiedStr = modifiedStr.Substring(0, index);
                modifiedStr += "]}";

                return JObject.Parse(modifiedStr);
            }
            catch { return null; }
        }

        private static T GetJsonValue<T>(JToken obj, List<int> indices)
        {
            try
            {
                foreach (var index in indices)
                {
                    obj = obj[index];
                }

                return obj.ToObject<T>();
            }
            catch (Exception e)
            {
                return default;
            }
        }

        private static string GetUsuallyWaitingTime(JToken token)
        {
            try
            {
                var currentHour = DateTime.Now.ToString("h tt", CultureInfo.InvariantCulture).ToLower();

                var openingHours = GetJsonValue<List<List<string>>>(token, new List<int> { 84, 0, (int)DateTime.Now.DayOfWeek, 1 });

                return openingHours.FirstOrDefault(h => h[4].Contains(currentHour))[5];
            }
            catch { return ""; }
        }

        private static List<OpeningHour> GetOpeningHours(JToken token)
        {
            try
            {
                List<OpeningHour> openingHours = new List<OpeningHour>();
                List<JToken> hours = GetJsonValue<List<JToken>>(token, new List<int> { 34, 1 });

                foreach (JToken t in hours)
                {
                    OpeningHour hour = new OpeningHour();
                    hour.DayOfWeek = t[0].ToString();

                    if (string.IsNullOrEmpty(t[1].ToString()) || t[1].ToString().Equals("[\r\n  \"Closed\"\r\n]"))
                    {
                        hour.OpensAt = DateTime.Parse("00:00", CultureInfo.InvariantCulture);
                        hour.CloseingAt = DateTime.Parse("00:00", CultureInfo.InvariantCulture);

                    }
                    else
                    {
                        var cleanTime = t[1].ToString().Replace("[", "").Replace("]", "").Replace("\"", "").Trim();

                        string[] times = cleanTime.Split(new String[] { "–" }, StringSplitOptions.None);
                        hour.OpensAt = DateTime.Parse(times[0], CultureInfo.InvariantCulture);
                        hour.CloseingAt = DateTime.Parse(times[1], CultureInfo.InvariantCulture);
                    }

                    openingHours.Add(hour);
                }

                return openingHours;
            }
            catch { return null; }
        }
        #endregion

        #region properties
        #endregion
    }
}
