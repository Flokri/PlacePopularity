using GeoCoordinatePortable;
using PlacePopularity.Exceptions;
using PlacePopularity.Logic;
using PlacePopularity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlacePopularity.Controller
{
    public class PlacePopularityInfo
    {
        #region instances
        private string _apiKey;
        private static PlacePopularityInfo _instance;

        private DateTime? _lastRequest = null;
        private int _lastRadius;
        private Location _lastLocation;

        private List<Place> _setOfNearbyPlaces;

        private int _failureRadius = 30;
        #endregion

        #region enum
        private enum RefreshType
        {
            complete,
            update,
            partlyNew
        }
        #endregion

        #region constructor
        private PlacePopularityInfo()
        {
            _setOfNearbyPlaces = new List<Place>();
        }
        #endregion

        #region publics
        public List<Place> GetNearbyPlaces(Location location, int radius)
        {
            if (radius == 0)
                throw new ArgumentException("Pleas specify a radius.");
            if (radius >= 25000)
                throw new ArgumentException("Please use a smaller radius.");
            if (location == null)
                throw new ArgumentException("Specify the current location of the user.");

            try
            {
                SetOfNearbyPlaces = LoadPlaces(location, radius, RefreshType.complete);
            }
            catch { }

            return SetOfNearbyPlaces;

        }
        #endregion

        #region privates

        private List<Place> LoadPlaces(Location location, int radius, RefreshType refreshType)
        {
            _lastRequest = DateTime.Now;
            _lastLocation = location;
            _lastRadius = radius;

            switch (refreshType)
            {
                case RefreshType.complete:
                    return NearbyPlaces.GetNearbyPlaces(_apiKey, location, radius, NearbyPlaces.PlaceType.grocery_or_supermarket);
                case RefreshType.update:
                    return NearbyPlaces.UpdateNearbyPlaces(_apiKey, location, radius, NearbyPlaces.PlaceType.grocery_or_supermarket, SetOfNearbyPlaces);
                case RefreshType.partlyNew:
                    return NearbyPlaces.UpdateNearbyPlaces(_apiKey, location, radius, NearbyPlaces.PlaceType.grocery_or_supermarket, SetOfNearbyPlaces);
                default:
                    return null;
            }
        }

        private bool LocationIsInsideFailureRadius(Location location)
        {
            var userCoordinate = new GeoCoordinate(location.lat, location.lng);
            var lastLocation = new GeoCoordinate(_lastLocation.lat, _lastLocation.lng);

            if (Math.Round(userCoordinate.GetDistanceTo(lastLocation), 0) <= _failureRadius)
                return true;
            return false;
        }
        #endregion 

        #region properties
        public static PlacePopularityInfo Instance
        {
            get => _instance = _instance ?? new PlacePopularityInfo();
        }

        public string ApiKey
        {
            set => _apiKey = value;
        }

        public List<Place> SetOfNearbyPlaces
        {
            get => _setOfNearbyPlaces;
            set => _setOfNearbyPlaces = value;
        }
        #endregion
    }
}
