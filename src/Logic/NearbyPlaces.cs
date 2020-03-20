using PlacePopularity.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using GeoCoordinatePortable;

namespace PlacePopularity.Logic
{
    public static class NearbyPlaces
    {
        #region consts
        private const string GOOGLE_BASE_URL = "https://maps.googleapis.com/maps/api/place/nearbysearch/json";
        private const string PLACE_SEARCH = "?location={0},{1}&radius={2}&type={3}&key={4}";
        #endregion

        #region enum
        public enum PlaceType
        {
            supermarket,
            grocery_or_supermarket,
        }
        #endregion

        #region publics
        public static List<Place> GetNearbyPlaces(string apiKey, Location location, int radius, PlaceType type)
        {
            try
            {
                string constructedUrl = GOOGLE_BASE_URL + string.Format(PLACE_SEARCH, location.lat.ToString().Replace(",", "."), location.lng.ToString().Replace(",", "."), radius, type.ToString(), apiKey);

                PlaceJson placesJson = HttpRequest.GetNearbyPlaceResponse(constructedUrl);
                return GetListOfPlaces(apiKey, placesJson, location);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<Place> UpdateNearbyPlaces(string apiKey, Location location, int radius, PlaceType type, List<Place> setOfNearbyPlaces)
        {
            try
            {
                string constructedUrl = GOOGLE_BASE_URL + string.Format(PLACE_SEARCH, location.lat.ToString().Replace(",", "."), location.lng.ToString().Replace(",", "."), radius, type.ToString(), apiKey);

                PlaceJson placesJson = HttpRequest.GetNearbyPlaceResponse(constructedUrl);
                return GetListOfPlaces(apiKey, placesJson, location, setOfNearbyPlaces);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion

        #region privates
        private static List<Place> GetListOfPlaces(string apiKey, PlaceJson response, Location location)
        {
            List<Place> places = new List<Place>();
            foreach (var item in response.results)
            {
                Place tmp = new Place()
                {
                    PlaceId = item.place_id,
                    PriceLevel = item.price_level,
                    Name = item.name,
                    Location = item.geometry.location,
                    IconUrl = item.icon,
                    Address = item.vicinity
                };

                try
                {
                    tmp.Popularity = HandlePlaceDetails.GetPopularity(apiKey, tmp.PlaceId);
                }
                catch { }

                var userCoordinate = new GeoCoordinate(location.lat, location.lng);
                var placeCoordinate = new GeoCoordinate(tmp.Location.lat, tmp.Location.lng);

                tmp.Distance = Math.Round(userCoordinate.GetDistanceTo(placeCoordinate), 2);

                places.Add(tmp);
            }

            return places;
        }
        private static List<Place> GetListOfPlaces(string apiKey, PlaceJson response, Location location, List<Place> setOfNearbyPlaces)
        {
            List<Place> places = new List<Place>();
            foreach (var item in response.results)
            {
                if (setOfNearbyPlaces.FirstOrDefault(p => p.Location.Equals(item.geometry.location)) == null)
                {
                    Place tmp = new Place()
                    {
                        PlaceId = item.place_id,
                        PriceLevel = item.price_level,
                        Name = item.name,
                        Location = item.geometry.location,
                        IconUrl = item.icon,
                        Address = item.vicinity
                    };

                    try
                    {
                        tmp.Popularity = HandlePlaceDetails.GetPopularity(apiKey, tmp.PlaceId);
                    }
                    catch { }

                    var userCoordinate = new GeoCoordinate(location.lat, location.lng);
                    var placeCoordinate = new GeoCoordinate(tmp.Location.lat, tmp.Location.lng);

                    tmp.Distance = Math.Round(userCoordinate.GetDistanceTo(placeCoordinate), 2);

                    places.Add(tmp);
                }
            }

            return places;
        }
        #endregion
    }
}
