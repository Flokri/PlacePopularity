using PlacePopularity.Controller;
using PlacePopularity.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlacePopularityTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string apiKey = "<API Key>";

            Location currentLocation = new Location()
            {
                lat = 0.0,
                lng = 0.0
            };

            int radius = 500; // radius of current Location in meter

            PlacePopularityInfo.Instance.ApiKey = apiKey;

            List<Place> places = PlacePopularityInfo.Instance.GetNearbyPlaces(currentLocation, radius);

            // sort the list by ascending distance
            List<Place> ascendingDistance = places.OrderBy(o => o.Distance).ToList();

            // get the popularity index of the nearby places
            string output = "";
            foreach (var item in ascendingDistance)
            {
                PopularityInfo info = PlacePopularityInfo.Instance.GetPopularity(item.PlaceId);

                if (info.CurrentPopularity == -1)
                {
                    output += $"Store:\t{info.Name}\n\t" +
                        $"No live data available!\n\t" +
                        $"Average Popularity at this time: " +
                        $"{info.OpeningHours.FirstOrDefault(h => h.DayOfWeek.Equals(DateTime.Now.DayOfWeek.ToString())).PopularityForHour.FirstOrDefault(h => h.Hour == DateTime.Now.Hour).Popularity}\n" +
                        $"-------------------------------------------------------------------------------------------------\n";
                }
                else
                {
                    output += $"Store:\t{item.Name}\n\t" +
                        $"Current popularity: {info.CurrentPopularity}\n\t" +
                        $"Usally waiting time at current time: {info.UsuallyWaitingTime}\n\t" +
                        $"Current popularity is {(info.CurrentPopularity * 100) / (info.OpeningHours.FirstOrDefault(h => h.DayOfWeek.Equals(DateTime.Now.DayOfWeek.ToString())).PopularityForHour.FirstOrDefault(h => h.Hour == DateTime.Now.Hour).Popularity)}% of the average for this day and time\n" +
                        $"-------------------------------------------------------------------------------------------------\n";
                    ;
                }

                output += "\n";
            }

            Console.WriteLine(output);
            Console.ReadKey();
        }
    }
}
