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
        #endregion

        #region constructor
        private PlacePopularityInfo() { }
        #endregion

        #region publics 
        public PopularityInfo GetPopularity(string placeId)
        {
            if (string.IsNullOrEmpty(_apiKey))
                throw new ApiKeyNotSetException("Set the API key before using the library.");
            if (_lastRequest?.Hour == DateTime.Now.Hour)
                throw new MultipleRequestInHour("The live update is only available once a hour.");

            try
            {
                PopularityInfo info = HandlePlaceDetails.GetPopularity(_apiKey, placeId);

                // disable for testing purposes
                //if (info != null)
                //    _lastRequest = DateTime.Now;

                return info;
            }
            catch { return null; }
        }
        #endregion

        #region privates
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
        #endregion
    }
}
