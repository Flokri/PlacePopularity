using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlacePopularity.Models
{
    public class PopularityInfo
    {
        #region properties
        /// <summary>
        /// The id of the place
        /// </summary>
        public string PlaceId { get; set; }

        /// <summary>
        /// The name of the place
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The current popularity of the place, if a live popularity is not available the value is -1
        /// </summary>
        public int CurrentPopularity { get; set; }

        /// <summary>
        /// If a live popularity is available the status desciribes how busy the place is right now
        /// </summary>
        public string CurrentPopularityStatus { get; set; }

        /// <summary>
        /// Get the average time people spent here
        /// </summary>
        public string AverageTimeSpent { get; set; }

        /// <summary>
        /// Get the usually waiting time at this place
        /// </summary>
        public string UsuallyWaitingTime { get; set; }

        /// <summary>
        /// The rating of the place
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// Get the total amount of ratings for this place
        /// </summary>
        public int TotalRatings { get; set; }

        /// <summary>
        /// Get the types that describes this place
        /// </summary>
        public List<string> Types { get; set; }

        public List<OpeningHour> OpeningHours { get; set; }

        public List<PopularityForDay> DailyPopularity { get; set; }

        public bool IsOpen => CheckIfThePlaceIsOpen();
        #endregion

        #region publics
        private bool CheckIfThePlaceIsOpen()
        {
            var day = OpeningHours.FirstOrDefault(d => d.DayOfWeek.Equals(DateTime.Now.DayOfWeek.ToString()));

            if ((day.OpensAt.TimeOfDay <= DateTime.Now.TimeOfDay) && (day.CloseingAt.TimeOfDay >= DateTime.Now.TimeOfDay))
                return true;

            return false;
        }
        #endregion
    }

    public class OpeningHour
    {
        #region properties
        public string DayOfWeek { get; set; }
        public DateTime OpensAt { get; set; }
        public DateTime CloseingAt { get; set; }
        public List<PopularityPerHour> PopularityForHour { get; set; }
        #endregion
    }

    public class PopularityForDay
    {
        public string DayOfWeek { get; set; }
        public List<PopularityPerHour> HourlyPopularity { get; set; }
    }
    public class PopularityPerHour
    {
        public int Hour { get; set; }
        public int Popularity { get; set; }
    }
}
