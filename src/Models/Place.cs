using System;
using System.Collections.Generic;
using System.Text;

namespace PlacePopularity.Models
{
    public class Place
    {
        #region instances
        /// <summary>
        /// The id of the place
        /// </summary>
        public string PlaceId { get; set; }

        /// <summary>
        /// The name of the place
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The exact location of the place (as longitude and latidute)
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// The absolute distance from the current user position to the place position
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// The price level of the place
        /// </summary>
        public int? PriceLevel { get; set; }

        /// <summary>
        /// The url to the icon for this place
        /// </summary>
        public string IconUrl { get; set; }
        #endregion
    }
}
