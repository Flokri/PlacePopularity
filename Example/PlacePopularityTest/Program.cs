using PlacePopularity.Controller;
using PlacePopularity.Models;
using System;
using System.Linq;

namespace PlacePopularityTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string apiKey = "<Enter API Key>";
            string placeId = "<Enter Place Id>";

            PlacePopularityInfo.Instance.ApiKey = apiKey;
            PopularityInfo info = PlacePopularityInfo.Instance.GetPopularity(placeId);

            if (info == null)
            {
                Console.WriteLine();
                Console.WriteLine("The api does get an error. Retry later");
            }

            // display the result to the user
            Console.WriteLine("The current popularity for the enterd place:");
            Console.WriteLine($"Name: {info.Name}");
            Console.WriteLine($"Current Popularity: {info.CurrentPopularity}");
            Console.WriteLine($"Current Popularity Status: {info.CurrentPopularityStatus}");
            Console.WriteLine($"Average Time Spent (per Person): {info.AverageTimeSpent}");
            Console.WriteLine($"Usually Waiting Time: {info.UsuallyWaitingTime}");
            Console.WriteLine($"Place Rating: {info.Rating}");
            Console.WriteLine($"Total Ratings: {info.TotalRatings}");
            Console.WriteLine($"Place Categories: {string.Join(" & ", info.Types)}");
            Console.WriteLine($"Is Open: {info.IsOpen}");
            Console.WriteLine($"Opening Hours:\n\t {string.Join("\n\t", info.OpeningHours.Select(x => x.DayOfWeek + ": " + x.OpensAt.TimeOfDay + " - " + x.CloseingAt.TimeOfDay).ToArray())}");

            Console.ReadKey();
        }
    }
}
